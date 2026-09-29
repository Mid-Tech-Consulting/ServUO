using System;
using Server;

namespace Server.Items
{
    public class BlackberryPumpkinPie : Food
    {
        public override int LabelNumber
        {
            get { return 1041348; } // baked pumpkin pie
        }

        [Constructable]
        public BlackberryPumpkinPie() : base(0x1041)
        {
            Name = "Blackberry Pumpkin Pie";
            Hue = 1175; // Rich blackberry purple hue
            Stackable = true;
            Weight = 1.0;
            FillFactor = 5;
        }

        public BlackberryPumpkinPie(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
