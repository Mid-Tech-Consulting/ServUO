using System;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseFruitTreeAddon : BaseAddon
    {
        private const int MaxFruits = 10;
        private int m_Fruits;
        private DateTime m_NextGrowth;

        public BaseFruitTreeAddon()
            : base()
        {
            m_Fruits = 1;
            m_NextGrowth = DateTime.Now + TimeSpan.FromDays(1);
            Timer.DelayCall(TimeSpan.FromDays(1), new TimerCallback(GrowFruit));
        }

        public BaseFruitTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override abstract BaseAddonDeed Deed { get; }
        public abstract Item Fruit { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Fruits
        {
            get { return m_Fruits; }
            set { m_Fruits = value < 0 ? 0 : value > MaxFruits ? MaxFruits : value; }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 2))
            {
                if (m_Fruits > 0)
                {
                    Item fruit = Fruit;

                    if (fruit == null)
                        return;

                    if (!from.PlaceInBackpack(fruit))
                    {
                        fruit.Delete();
                        from.SendLocalizedMessage(501015); // There is no room in your backpack for the fruit.
                    }
                    else
                    {
                        --m_Fruits;
                        from.SendLocalizedMessage(501016); // You pick some fruit and put it in your backpack.
                    }
                }
                else
                    from.SendLocalizedMessage(501017); // There is no more fruit on this tree
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write((int)m_Fruits);
            writer.Write(m_NextGrowth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Fruits = reader.ReadInt();

            if (version >= 1)
                m_NextGrowth = reader.ReadDateTime();
            else
                m_NextGrowth = DateTime.Now + TimeSpan.FromDays(1);

            TimeSpan delay = m_NextGrowth - DateTime.Now;
            Timer.DelayCall(delay > TimeSpan.Zero ? delay : TimeSpan.Zero, new TimerCallback(GrowFruit));
        }

        private void GrowFruit()
        {
            if (Deleted)
                return;

            if (m_Fruits < MaxFruits)
                m_Fruits++;

            m_NextGrowth = DateTime.Now + TimeSpan.FromDays(1);
            Timer.DelayCall(TimeSpan.FromDays(1), new TimerCallback(GrowFruit));
        }
    }

    public class AppleTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public AppleTreeAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xD98, 1076269), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x3124, 1076269), 0, 0, 0);
        }

        public AppleTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AppleTreeDeed();
            }
        }
        public override Item Fruit
        {
            get
            {
                return new Apple();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class AppleTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public AppleTreeDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public AppleTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AppleTreeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076269;
            }
        }// Apple Tree
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PeachTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public PeachTreeAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xD9C, 1076270), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x3123, 1076270), 0, 0, 0);
        }

        public PeachTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new PeachTreeDeed();
            }
        }
        public override Item Fruit
        {
            get
            {
                return new Peach();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PeachTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public PeachTreeDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public PeachTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new PeachTreeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076270;
            }
        }// Peach Tree
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PlumTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public PlumTreeAddon()
            : base()
        {
            AddComponent(new LocalizedAddonComponent(0x9E38, 1029965), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x9E39, 1029965), 0, 0, 0);
        }

        public PlumTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new PlumTreeDeed(); } }
        public override Item Fruit { get { return new Plum(); } }

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

    public class PlumTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public PlumTreeDeed()
            : base()
        {
        }

        public PlumTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1157312; } } // Plum Tree
        public override BaseAddon Addon { get { return new PlumTreeAddon(); } }

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
}