using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class HalloweenEventPumpkin : Item
    {
        [Constructable]
        public HalloweenEventPumpkin() : this(1)
        {
        }

        [Constructable]
        public HalloweenEventPumpkin(int amount) : base(0x4694)
        {
            Stackable = true;
            Weight = 0.05;
            Amount = amount;
            Hue = 0;
            Name = "Halloween Pumpkin";
            LootType = LootType.Regular;
        }

        public HalloweenEventPumpkin(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("<BASEFONT COLOR=#FFA500>Halloween Event Currency</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Redeem at Cemetery Reward Stones or Jack the Pumpkin Carver</BASEFONT>");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendMessage(0x35, "You have {0} Halloween Pumpkin{1}. Slay monsters in cemeteries to collect more, then redeem them at a Cemetery Reward Stone or Jack the Pumpkin Carver!", 
                    from.Backpack.GetAmount(typeof(HalloweenEventPumpkin)),
                    from.Backpack.GetAmount(typeof(HalloweenEventPumpkin)) == 1 ? "" : "s");
            }
            else
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
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
