using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class EtherealOrangeSpider : EtherealMount
    {
        [Constructable]
        public EtherealOrangeSpider()
            : base(0x9DD6, 0x3ECA, 0x3ECA, 1161, 1161)
        {
            Name = "Ethereal Orange Spider";
            Hue = 1161;
            StatueHue = 1161;
            NonTransparentMountedHue = 1161;
            TransparentMountedHue = 1161;
            Transparent = false;
            LootType = LootType.Blessed;
        }

        public EtherealOrangeSpider(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event Rare Mount</BASEFONT>");
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
