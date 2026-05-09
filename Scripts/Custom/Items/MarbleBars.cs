using System;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    /// <summary>
    /// Decorative blessed marble bar (graphic 0x99C8).
    /// </summary>
    public class MarbleBar : Item
    {
        [Constructable]
        public MarbleBar()
            : base(0x99C8)
        {
            Name = "Marble Bar";
            Weight = 5.0;
            LootType = LootType.Blessed;
        }

        public MarbleBar(Serial serial)
            : base(serial)
        {
        }

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

    /// <summary>
    /// Storage version of the marble bar (graphic 0x99C9). 125-item capacity,
    /// 400-stone weight cap matching a standard wooden chest. Implements
    /// ISecurable so it can be locked down in a house with per-level access
    /// (Owner / Co-Owner / Friend / Anyone).
    /// </summary>
    public class MarbleBarContainer : Container, ISecurable
    {
        public override int DefaultMaxItems { get { return 125; } }
        public override int DefaultMaxWeight { get { return 400; } }

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [Constructable]
        public MarbleBarContainer()
            : base(0x99C9)
        {
            Name = "Marble Bar";
            Weight = 5.0;
            m_Level = SecureLevel.CoOwners;
        }

        public MarbleBarContainer(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        // Mirrors the pattern used by other ISecurable containers (SeedBox).
        // GMs always pass; outside a house the SecureLevel doesn't apply;
        // inside a house it's gated by the standard HasSecureAccess rules.
        private bool CheckAccessible(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (IsChildOf(from.Backpack))
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null)
                return true;

            return house.HasSecureAccess(from, m_Level);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CheckAccessible(from))
            {
                from.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                return;
            }

            base.OnDoubleClick(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CheckAccessible(from))
            {
                from.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!CheckAccessible(from))
            {
                from.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                return false;
            }

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (!CheckAccessible(from))
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return base.CheckLift(from, item, ref reject);
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

            if (version >= 1)
                m_Level = (SecureLevel)reader.ReadEncodedInt();
            else
                m_Level = SecureLevel.CoOwners;
        }
    }
}
