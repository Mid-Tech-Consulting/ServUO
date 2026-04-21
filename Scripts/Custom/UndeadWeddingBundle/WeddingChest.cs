using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x9F94, 0x9F95)]
    public class WeddingChest : BaseContainer
    {
        public override int LabelNumber { get { return 1124876; } } // Wedding Chest

        [Constructable]
        public WeddingChest()
            : base(0x9F94)
        {
            this.Weight = 2.0;
            this.GumpID = 0x266A;

            LootType = LootType.Blessed;
        }

        public WeddingChest(Serial serial)
            : base(serial)
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
}
