using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class MonthlyBeardDye : Item
    {
        public override int LabelNumber { get { return 1071387; } } // Natural Hair Dye

        [Constructable]
        public MonthlyBeardDye() : this(0)
        {
        }

        [Constructable]
        public MonthlyBeardDye(int hue) : base(0xEFE)
        {
            Weight = 1.0;
            Hue = hue;
            Name = "Monthly Beard Dye";
        }

        public MonthlyBeardDye(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                BaseGump.SendGump(new BeardDyeConfirmGump(m as PlayerMobile, Hue, this));
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

    public class BeardDyeConfirmGump : BaseGump
    {
        public int Hue { get; private set; }
        public Item Dye { get; private set; }

        public BeardDyeConfirmGump(PlayerMobile pm, int hue, Item dye)
            : base(pm, 200, 200)
        {
            Hue = hue;
            Dye = dye;

            pm.CloseGump(typeof(BeardDyeConfirmGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 340, 200, 3600);
            AddBackground(0, 0, 100, 100, 3600);
            AddBackground(100, 140, 227, 50, 9270);

            AddImage(-39, -23, 50702, Dye.Hue - 1);

            AddHtmlLocalized(110, 25, 205, 80, 1074396, C32216(0x0080FF), false, false); // This special hair dye is made of a unique mixture of leaves, permanently changing one's hair color until another dye is used.
            AddHtmlLocalized(120, 155, 160, 20, 1074395, C32216(0x00FFFF), false, false); // <div align=right>Use Permanent Hair Dye</div>

            AddButton(290, 157, 0x7538, 0x7539, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            if (Dye.Deleted || !Dye.IsChildOf(User.Backpack))
            {
                User.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
                return;
            }

            if (User.FacialHairItemID == 0)
            {
                User.SendLocalizedMessage(502623); // You have no hair to dye and you cannot use this.
                return;
            }

            User.FacialHairHue = Hue;
            User.SendLocalizedMessage(501199); // You dye your hair
            Dye.Delete();
            User.PlaySound(0x4E);

            Item beard = User.FindItemOnLayer(Layer.FacialHair);
            if (beard != null)
            {
                if (Dye.Name != null && Dye.Name.Contains("4th of July"))
                {
                    beard.Name = "Beard (4th of July)";
                    new PatrioticTimer(beard).Start();
                }
                else
                {
                    beard.Name = null;
                }
            }
        }
    }
}
