/* Created by Hammerhand & Milva */

using System;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Engines.Harvest;

namespace Server.Items
{

    public class GoldPan : BaseHarvestTool, IUsesRemaining
    {
        public override HarvestSystem HarvestSystem { get { return GoldPanningSystem.Instance; } }

        [Constructable]
        public GoldPan(): this(10)
        {
            Name = "a Gold Pan";
        }
        [Constructable]
        public GoldPan(int uses): base(0x9D7)
        {
            Name = "a Gold Pan";
            Weight = 2.0;
            Hue = 2503;
            Movable = true;
            UsesRemaining = uses;
            ShowUsesRemaining = true;
        }

        public GoldPan(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}/* Created by Hammerhand & Milva */

using System;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Engines.Harvest;

namespace Server.Items
{

    public class GoldPan : Item, IUsesRemaining
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        [Constructable]
        public GoldPan(): this(10)
        {
            Name = "a Gold Pan";
        }

        [Constructable]
        public GoldPan(int uses): base(0x9D7)
        {
            Name = "a Gold Pan";
            Weight = 2.0;
            Hue = 2503;
            Movable = true;
            UsesRemaining = uses;
            ShowUsesRemaining = true;
        }

        public GoldPan(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
                GoldPanningSystem.Instance.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (ShowUsesRemaining)
                list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    m_UsesRemaining = reader.ReadInt();
                    break;
                }
            }
        }
    }
}