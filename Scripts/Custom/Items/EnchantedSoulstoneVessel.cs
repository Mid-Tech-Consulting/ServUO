using System;

using Server;
using Server.Network;

namespace Server.Items
{
    /// <summary>
    /// Account-bound container that holds up to 20 soulstones. Sold from the
    /// Ultima Store for 1000 sovereigns; bound to the purchasing account on
    /// construction so only that account can open or modify it.
    /// </summary>
    public class EnchantedSoulstoneVessel : Container, IAccountRestricted
    {
        public override int LabelNumber { get { return 1158405; } } // close enough placeholder; falls back to Name

        // Soulstone capacity. Container is otherwise blessed and weightless to
        // the holder once equipped/in pack -- only the static 30-stone weight
        // applies via the override below.
        public const int Capacity = 20;

        private string m_Account;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account
        {
            get { return m_Account; }
            set { m_Account = value; InvalidateProperties(); }
        }

        public override int DefaultMaxItems { get { return Capacity; } }
        public override int DefaultMaxWeight { get { return 0; } } // ignore -- soulstones are blessed/weightless

        [Constructable]
        public EnchantedSoulstoneVessel()
            : base(0xA73F)
        {
            Weight = 30.0;
            LootType = LootType.Blessed;
            Name = "Enchanted Soulstone Vessel";
        }

        public EnchantedSoulstoneVessel(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D point)
        {
            if (!CheckAccess(from))
            {
                from.SendMessage(0x22, "This vessel is bound to another account.");
                return false;
            }

            if (!(dropped is SoulStone))
            {
                from.SendMessage(0x22, "Only soulstones may be placed inside this vessel.");
                return false;
            }

            if (Items.Count >= Capacity)
            {
                from.SendMessage(0x22, "This vessel is full.");
                return false;
            }

            return base.OnDragDropInto(from, dropped, point);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CheckAccess(from))
            {
                from.SendMessage(0x22, "This vessel is bound to another account.");
                return false;
            }

            if (!(dropped is SoulStone))
            {
                from.SendMessage(0x22, "Only soulstones may be placed inside this vessel.");
                return false;
            }

            if (Items.Count >= Capacity)
            {
                from.SendMessage(0x22, "This vessel is full.");
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (!CheckAccess(from))
            {
                reject = LRReason.CannotLift;
                from.SendMessage(0x22, "This vessel is bound to another account.");
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CheckAccess(from))
            {
                from.SendMessage(0x22, "This vessel is bound to another account.");
                return;
            }

            base.OnDoubleClick(from);
        }

        private bool CheckAccess(Mobile from)
        {
            if (string.IsNullOrEmpty(m_Account))
                return true;

            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return from.Account != null && from.Account.Username == m_Account;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(m_Account))
            {
                list.Add(1041602, m_Account); // Owner: ~1_val~
                list.Add("Account Bound");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Account = reader.ReadString();
        }
    }
}
