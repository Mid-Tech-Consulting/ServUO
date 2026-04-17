using Server.Mobiles;

namespace Server.Items
{
    public class EtherealHellfireSteed : EtherealMount
    {
        [Constructable]
        public EtherealHellfireSteed()
            : base(0xB165, 0x3EE0, 0x3EA0)
        {
            Name = "Ethereal Hellfire Steed";
            Hue = 1161;
        }

        public EtherealHellfireSteed(Serial serial)
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
