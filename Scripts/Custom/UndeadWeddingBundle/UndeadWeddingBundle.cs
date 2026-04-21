using System;

namespace Server.Items
{
    public class UndeadWeddingBundle : Backpack
    {
        public override int LabelNumber { get { return 1157895; } } // Undead Wedding Bundle

        [Constructable]
        public UndeadWeddingBundle()
        {
            WeddingChest box = new WeddingChest();

            box.DropItem(new UndeadWeddingHat());
            box.DropItem(new UndeadWeddingVeil());

            DropItem(box);
        }

        public UndeadWeddingBundle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
