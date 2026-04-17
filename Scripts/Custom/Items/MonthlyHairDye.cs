using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class MonthlyHairDye : Item
    {
        public override int LabelNumber { get { return 1071387; } } // Natural Hair Dye

        [Constructable]
        public MonthlyHairDye() : this(0)
        {
        }

        [Constructable]
        public MonthlyHairDye(int hue) : base(0xEFE)
        {
            Weight = 1.0;
            Hue = hue;
        }

        public MonthlyHairDye(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                BaseGump.SendGump(new HairDyeConfirmGump(m as PlayerMobile, Hue, this));
            }
            else
            {
                m.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
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
            reader.ReadInt();
        }
    }
}
