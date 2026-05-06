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
	public class CrystalPortal : Item, ISecurable
	{
        public override int LabelNumber { get { return 1113945; } } // Crystal Portal

		private SecureLevel m_Level;

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level
		{
			get { return m_Level; }
			set { m_Level = value; }
		}

		public override bool HandlesOnSpeech { get { return true; } }

		[Constructable]
		public CrystalPortal()
            : base(0x468B)
		{
			Weight = 5.0;
			Movable = true;
			LootType = LootType.Blessed;
		}

		public CrystalPortal(Serial serial)
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
				m.SendGump(new CrystalPortalGump(m, this));
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
				if (SpellHelper.CheckTravel(m, map, loc, TravelCheckType.RecallTo))
				{
					OnTeleport(m, loc, map);
				}
				else
				{
					m.LocalOverheadMessage(MessageType.Regular, 0x4F1, 502360); // You cannot teleport into that area.
				}
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
                ItemID = 0x468B;
                Hue = 0;
                Weight = 5.0;
            }
        }

		#region Destination Tables

		public class Destination
		{
			public string SpeechKey { get; }
			public string Label { get; }
			public Point3D Location { get; }
			public Map Map { get; }

			public Destination(string speechKey, string label, Point3D location, Map map)
			{
				SpeechKey = speechKey;
				Label = label;
				Location = location;
				Map = map;
			}
		}

		// Ordered by facet group; speech keys preserved verbatim from the legacy switch
		// so existing player macros / muscle memory keep working.
		// Trammel and Ilshenar destinations intentionally omitted — this shard does not
		// support those facets / they have no useful endpoints.
		public static readonly Destination[] AllDestinations = new[]
		{
			// Felucca banks
			new Destination("fel britain mint", "Britain Mint", new Point3D(1434, 1699, 2), Map.Felucca),
			new Destination("fel bucs mint", "Bucs Mint", new Point3D(2724, 2192, 0), Map.Felucca),
			new Destination("fel cove mint", "Cove Mint", new Point3D(2238, 1195, 0), Map.Felucca),
			new Destination("fel jhelom mint", "Jhelom Mint", new Point3D(1331, 3779, 0), Map.Felucca),
			new Destination("fel magincia mint", "Magincia Mint", new Point3D(3728, 2164, 20), Map.Felucca),
			new Destination("fel minoc mint", "Minoc Mint", new Point3D(2498, 561, 0), Map.Felucca),
			new Destination("fel moonglow mint", "Moonglow Mint", new Point3D(4471, 1177, 0), Map.Felucca),
			new Destination("fel nujelm mint", "Nujelm Mint", new Point3D(3770, 1308, 0), Map.Felucca),
			new Destination("fel ocllo mint", "Ocllo Mint", new Point3D(3687, 2523, 0), Map.Felucca),
			new Destination("fel serpent mint", "Serpent Mint", new Point3D(2895, 3479, 15), Map.Felucca),
			new Destination("fel skara mint", "Skara Mint", new Point3D(596, 2138, 0), Map.Felucca),
			new Destination("fel trinsic mint", "Trinsic Mint", new Point3D(1823, 2821, 0), Map.Felucca),
			new Destination("fel vesper mint", "Vesper Mint", new Point3D(2899, 676, 0), Map.Felucca),
			new Destination("fel wind mint", "Wind Mint", new Point3D(1361, 895, 0), Map.Felucca),
			new Destination("fel yew mint", "Yew Mint", new Point3D(643, 858, 0), Map.Felucca),

			// Felucca moongates
			new Destination("fel britain moongate", "Britain Moongate", new Point3D(1336, 1997, 5), Map.Felucca),
			new Destination("fel bucs moongate", "Bucs Moongate", new Point3D(2711, 2234, 0), Map.Felucca),
			new Destination("fel jhelom moongate", "Jhelom Moongate", new Point3D(1495, 3773, 0), Map.Felucca),
			new Destination("fel magincia moongate", "Magincia Moongate", new Point3D(3563, 2139, 34), Map.Felucca),
			new Destination("fel minoc moongate", "Minoc Moongate", new Point3D(2701, 692, 5), Map.Felucca),
			new Destination("fel moonglow moongate", "Moonglow Moongate", new Point3D(4467, 1283, 5), Map.Felucca),
			new Destination("fel skara moongate", "Skara Moongate", new Point3D(643, 2067, 5), Map.Felucca),
			new Destination("fel trinsic moongate", "Trinsic Moongate", new Point3D(1828, 2948, -20), Map.Felucca),
			new Destination("fel vesper moongate", "Vesper Moongate", new Point3D(2701, 692, 5), Map.Felucca),
			new Destination("fel yew moongate", "Yew Moongate", new Point3D(771, 752, 5), Map.Felucca),

			// Malas
			new Destination("luna mint", "Luna Mint", new Point3D(1015, 527, -65), Map.Malas),
			new Destination("umbra mint", "Umbra Mint", new Point3D(2047, 1353, -85), Map.Malas),
			new Destination("luna moongate", "Luna Moongate", new Point3D(1015, 527, -65), Map.Malas),
			new Destination("umbra moongate", "Umbra Moongate", new Point3D(1997, 1386, -85), Map.Malas),

			// Tokuno
			new Destination("zento mint", "Zento Mint", new Point3D(741, 1261, 30), Map.Tokuno),
			new Destination("isamu moongate", "Isamu-Jima Moongate", new Point3D(1169, 998, 41), Map.Tokuno),
			new Destination("makoto moongate", "Makoto-Jima Moongate", new Point3D(802, 1204, 25), Map.Tokuno),
			new Destination("homare moongate", "Homare-Jima Moongate", new Point3D(270, 628, 15), Map.Tokuno),

			// TerMur
			new Destination("royal mint", "Royal City Mint", new Point3D(842, 3451, -20), Map.TerMur),
			new Destination("termur moongate", "Ter Mur Moongate", new Point3D(852, 3526, -43), Map.TerMur),
		};

		public static IEnumerable<Destination> GetDestinations(Map facet)
		{
			foreach (Destination d in AllDestinations)
				if (d.Map == facet)
					yield return d;
		}

		#endregion

		public static void ResolveDest(Mobile from, string name, ref Point3D loc, ref Map map)
		{
			if (String.IsNullOrWhiteSpace(name))
				return;

			string key = name.Trim().ToLower();

			foreach (Destination d in AllDestinations)
			{
				if (d.SpeechKey == key)
				{
					loc = d.Location;
					map = d.Map;
					return;
				}
			}
		}
	}
}
