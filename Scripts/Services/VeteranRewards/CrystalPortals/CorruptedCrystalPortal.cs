#region References
using System;
using System.Collections.Generic;

using Server.Factions;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Multis;
using Server.ContextMenus;
#endregion

namespace Server.Items
{
	public class CorruptedCrystalPortal : Item, ISecurable
	{
        public override int LabelNumber { get { return 1150074; } } // Corrupted Crystal Portal

        private SecureLevel m_Level;

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level
		{
			get { return m_Level; }
			set { m_Level = value; }
		}

		public override bool HandlesOnSpeech { get { return true; } }

		[Constructable]
		public CorruptedCrystalPortal()
            : base(0x468A)
		{
            Hue = 2601;
			Weight = 1.0;
			Movable = true;
			LootType = LootType.Blessed;
		}

		public CorruptedCrystalPortal(Serial serial)
			: base(serial)
		{ }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

		public virtual bool ValidateUse(Mobile m, bool message)
		{
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || !IsLockedDown)
            {
                if (message)
                {
                    m.SendMessage("This must be locked down in a house to use!");
                }

                return false;
            }

            if (!house.HasSecureAccess(m, m_Level))
            {
                if (message)
                {
                    m.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                }

                return false;
            }

			if (Sigil.ExistsOn(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
				}

				return false;
			}

			if (WeightOverloading.IsOverloaded(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
				}

				return false;
			}

			if (m.Criminal)
			{
				if (message)
				{
					m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
				}

				return false;
			}

			if (SpellHelper.CheckCombat(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				}

				return false;
			}

			if (m.Spell != null)
			{
				if (message)
				{
					m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
				}

				return false;
			}

            if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(m))
            {
                if (message)
                {
                    m.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                }

                return false;
            }

			return true;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!m.InRange(Location, 3))
			{
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			if (ValidateUse(m, true))
			{
				m.SendGump(new CorruptedCrystalPortalGump(m, this));
			}
		}

		public virtual void OnTeleport(Mobile m, Point3D loc, Map map)
		{
			if (m == null || loc == Point3D.Zero || map == null || map == Map.Internal)
			{
				return;
			}

			Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
			Effects.PlaySound(m.Location, m.Map, 0x1FE);

			BaseCreature.TeleportPets(m, loc, map);
			m.MoveToWorld(loc, map);

			Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
			Effects.PlaySound(m.Location, m.Map, 0x1FE);
		}

		// Routes a destination selection (from speech or gump) through validation,
		// travel restrictions, and then triggers the teleport.
		public void TryTeleport(Mobile m, Point3D loc, Map map)
		{
			if (loc == Point3D.Zero || map == null || map == Map.Internal || (Siege.SiegeShard && map == Map.Trammel))
			{
				return;
			}

			if (SpellHelper.RestrictRedTravel && !Siege.SiegeShard && m.Murderer && map != Map.Felucca)
			{
				m.SendLocalizedMessage(1019004); // You are not allowed to travel there.
				return;
			}

			if (ValidateUse(m, true))
			{
				OnTeleport(m, loc, map);
			}
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (e.Handled || e.Blocked || !e.Mobile.InRange(Location, 2))
			{
				return;
			}

			Point3D loc = Point3D.Zero;
			Map map = null;

			ResolveDest(e.Mobile, e.Speech.Trim(), ref loc, ref map);

			if (loc == Point3D.Zero || map == null)
			{
				return;
			}

			e.Handled = true;
			TryTeleport(e.Mobile, loc, map);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(1); // version

			writer.WriteEncodedInt((int)m_Level);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Level = (SecureLevel)reader.ReadEncodedInt();

            if (version < 1)
            {
                ItemID = 0x468A;
                Hue = 2601;
            }
		}

		#region Destination Tables

		public class Destination
		{
			public string SpeechKey { get; }
			public string Label { get; }
			public Point3D Location { get; }
			public Map Map { get; }
			public bool RequireAbyssEntry { get; }

			public Destination(string speechKey, string label, Point3D location, Map map, bool requireAbyssEntry = false)
			{
				SpeechKey = speechKey;
				Label = label;
				Location = location;
				Map = map;
				RequireAbyssEntry = requireAbyssEntry;
			}
		}

		// Speech keys preserved verbatim from the legacy switch.
		// Trammel destinations intentionally omitted — this shard does not support Trammel.
		public static readonly Destination[] AllDestinations = new[]
		{
			// Felucca ruleset
			new Destination("fel dungeon covetous", "Covetous", new Point3D(2498, 921, 0), Map.Felucca),
			new Destination("fel dungeon deceit", "Deceit", new Point3D(4111, 434, 5), Map.Felucca),
			new Destination("fel dungeon despise", "Despise", new Point3D(1301, 1080, 0), Map.Felucca),
			new Destination("fel dungeon destard", "Destard", new Point3D(1176, 2640, 2), Map.Felucca),
			new Destination("fel dungeon fire", "Fire", new Point3D(2923, 3409, 8), Map.Felucca),
			new Destination("fel dungeon hythloth", "Hythloth", new Point3D(4721, 3824, 0), Map.Felucca),
			new Destination("fel dungeon ice", "Ice", new Point3D(1999, 81, 4), Map.Felucca),
			new Destination("fel dungeon orc", "Orc Cave", new Point3D(1017, 1429, 0), Map.Felucca),
			new Destination("fel dungeon shame", "Shame", new Point3D(511, 1565, 0), Map.Felucca),
			new Destination("fel dungeon wrong", "Wrong", new Point3D(2043, 238, 10), Map.Felucca),
			new Destination("fel dungeon wind", "Wind", new Point3D(1361, 895, 0), Map.Felucca),
			new Destination("fel dungeon prism", "Prism of Light", new Point3D(3786, 1095, 18), Map.Felucca),
			new Destination("fel dungeon sanctuary", "Sanctuary", new Point3D(761, 1644, 0), Map.Felucca),
			new Destination("fel dungeon palace", "Palace of Paroxysmus", new Point3D(5624, 3040, 13), Map.Felucca),
			new Destination("fel dungeon grove", "Twisted Weald", new Point3D(580, 1655, 0), Map.Felucca),
			new Destination("fel dungeon caves", "Painted Caves", new Point3D(1717, 2991, 0), Map.Felucca),
			new Destination("fel dungeon blackthorn", "Blackthorn's Castle", new Point3D(1520, 1418, 15), Map.Felucca),

			// Malas
			new Destination("dungeon doom", "Doom", new Point3D(2368, 1267, -85), Map.Malas),
			new Destination("dungeon bedlam", "Bedlam", new Point3D(2068, 1372, -75), Map.Malas),
			new Destination("dungeon labyrinth", "Labyrinth", new Point3D(1732, 975, -75), Map.Malas),

			// Tokuno
			new Destination("dungeon citadel", "Citadel", new Point3D(1345, 769, 19), Map.Tokuno),
			new Destination("dungeon fandancer", "Fan Dancer Dojo", new Point3D(970, 222, 23), Map.Tokuno),
			new Destination("dungeon mines", "Yomotsu Mines", new Point3D(257, 786, 63), Map.Tokuno),

			// TerMur
			new Destination("dungeon underworld", "Underworld", new Point3D(1128, 1207, -2), Map.TerMur),
			new Destination("dungeon abyss", "Stygian Abyss", new Point3D(946, 71, 72), Map.TerMur, requireAbyssEntry: true),
		};

		#endregion

		public static void ResolveDest(Mobile m, string name, ref Point3D loc, ref Map map)
		{
			if (String.IsNullOrWhiteSpace(name))
				return;

			string key = name.Trim().ToLower();

			foreach (Destination d in AllDestinations)
			{
				if (d.SpeechKey == key)
				{
					if (d.RequireAbyssEntry)
					{
						PlayerMobile pm = m as PlayerMobile;
						if (pm == null || !pm.AbyssEntry)
						{
							m.SendLocalizedMessage(1112226);
							return;
						}
					}

					loc = d.Location;
					map = d.Map;
					return;
				}
			}
		}
	}
}
