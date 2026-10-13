using System;
using Server;

namespace Server.Items
{
    public class TurkeyFeather : Item
    {
        [Constructable]
        public TurkeyFeather() : this(1)
        {
        }

        [Constructable]
        public TurkeyFeather(int amount) : base(0x1BD1)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1161; // Warm Autumn Blaze
            Weight = 0.01;
        }

        public TurkeyFeather(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1027151; // feather

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Amount > 1)
            {
                list.Add(String.Format("{0:N0} turkey feathers", Amount));
            }
            else
            {
                list.Add("turkey feather");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>November Event Currency</BASEFONT>");
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
