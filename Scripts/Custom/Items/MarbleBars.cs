using System;

using Server;

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
    /// 400-stone weight cap matching a standard wooden chest.
    /// </summary>
    public class MarbleBarContainer : Container
    {
        public override int DefaultMaxItems { get { return 125; } }
        public override int DefaultMaxWeight { get { return 400; } }

        [Constructable]
        public MarbleBarContainer()
            : base(0x99C9)
        {
            Name = "Marble Bar";
            Weight = 5.0;
        }

        public MarbleBarContainer(Serial serial)
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
}
