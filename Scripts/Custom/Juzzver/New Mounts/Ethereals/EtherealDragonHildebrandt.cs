using Server.Mobiles;

namespace Server.Items
{
    public class EtherealDragonHildebrandt : EtherealMount
    {
        [Constructable]
        public EtherealDragonHildebrandt()
            : base(0xB162, 0x3EDD, 0x3EDD)
        {
            Name = "Ethereal Dragon Hildebrandt";
        }

        public EtherealDragonHildebrandt(Serial serial)
            : base(serial)
        { }

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
