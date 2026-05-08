using System;
using System.Collections.Generic;

using Server;
using Server.Factions;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    /// <summary>
    /// Devotion-themed teleport ankh: double-click to open a destination
    /// picker and warp to a Felucca virtue shrine or one of the three Tokuno
    /// island shrines. Same travel-validation flow as the Crystal Portal
    /// (criminal / encumber / combat / spell / trade-order checks).
    /// </summary>
    public class AnkhOfDevotion : Item
    {
        [Constructable]
        public AnkhOfDevotion()
            : base(0x99C7)
        {
            Name = "Ankh Of Devotion";
            Hue = 0x0AC0;
            Weight = 10.0;
            LootType = LootType.Blessed;
            Movable = true;
        }

        public AnkhOfDevotion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (ValidateUse(m, true))
            {
                if (m is PlayerMobile pm)
                {
                    pm.CloseGump(typeof(AnkhOfDevotionGump));
                    pm.SendGump(new AnkhOfDevotionGump(pm, this));
                }
            }
        }

        public virtual bool ValidateUse(Mobile m, bool message)
        {
            if (Sigil.ExistsOn(m))
            {
                if (message)
                    m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }

            if (WeightOverloading.IsOverloaded(m))
            {
                if (message)
                    m.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            if (m.Criminal)
            {
                if (message)
                    m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }

            if (SpellHelper.CheckCombat(m))
            {
                if (message)
                    m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }

            if (m.Spell != null)
            {
                if (message)
                    m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                return false;
            }

            if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(m))
            {
                if (message)
                    m.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return false;
            }

            return true;
        }

        public virtual void OnTeleport(Mobile m, Point3D loc, Map map)
        {
            if (m == null || loc == Point3D.Zero || map == null || map == Map.Internal)
                return;

            Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            Effects.PlaySound(m.Location, m.Map, 0x1FE);

            BaseCreature.TeleportPets(m, loc, map);
            m.MoveToWorld(loc, map);

            Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            Effects.PlaySound(m.Location, m.Map, 0x1FE);
        }

        public void TryTeleport(Mobile m, Point3D loc, Map map)
        {
            if (loc == Point3D.Zero || map == null || map == Map.Internal || (Siege.SiegeShard && map == Map.Trammel))
                return;

            if (SpellHelper.RestrictRedTravel && !Siege.SiegeShard && m.Murderer && map != Map.Felucca)
            {
                m.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return;
            }

            if (ValidateUse(m, true))
            {
                if (SpellHelper.CheckTravel(m, map, loc, TravelCheckType.RecallTo))
                    OnTeleport(m, loc, map);
                else
                    m.LocalOverheadMessage(MessageType.Regular, 0x4F1, 502360); // You cannot teleport into that area.
            }
        }

        #region Destination Tables

        public class Destination
        {
            public string Label { get; }
            public Point3D Location { get; }
            public Map Map { get; }

            public Destination(string label, Point3D location, Map map)
            {
                Label = label;
                Location = location;
                Map = map;
            }
        }

        // Felucca virtue shrines + Chaos shrine. Coordinates lifted from
        // Scripts/Services/Factions/Items/Equipment/ShrineGem.cs which
        // provides the authoritative ServUO list.
        public static readonly Destination[] FeluccaShrines = new[]
        {
            new Destination("Shrine of Compassion", new Point3D(1857, 865, -1), Map.Felucca),
            new Destination("Shrine of Honesty", new Point3D(4220, 563, 36), Map.Felucca),
            new Destination("Shrine of Honor", new Point3D(1732, 3528, 0), Map.Felucca),
            new Destination("Shrine of Humility", new Point3D(4264, 3707, 0), Map.Felucca),
            new Destination("Shrine of Justice", new Point3D(1300, 644, 8), Map.Felucca),
            new Destination("Shrine of Sacrifice", new Point3D(3355, 302, 9), Map.Felucca),
            new Destination("Shrine of Spirituality", new Point3D(1606, 2490, 5), Map.Felucca),
            new Destination("Shrine of Valor", new Point3D(2500, 3931, 3), Map.Felucca),
            new Destination("Shrine of Chaos", new Point3D(1470, 843, 0), Map.Felucca),
        };

        // Tokuno island shrines (Homare/Isamu/Makoto moongate spawns).
        public static readonly Destination[] TokunoShrines = new[]
        {
            new Destination("Shrine of Homare", new Point3D(287, 711, 54), Map.Tokuno),
            new Destination("Shrine of Isamu", new Point3D(1044, 517, 15), Map.Tokuno),
            new Destination("Shrine of Makoto", new Point3D(718, 1162, 25), Map.Tokuno),
        };

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
