using System;
using Server;
using Server.Custom.Events.HalloweenCemetery;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class HalloweenEventBag : Item
    {
        [Constructable]
        public HalloweenEventBag() : base(0xE76)
        {
            Name = "Halloween Event Bag";
            Hue = 1161; // Candy Corn Orange
            LootType = LootType.Blessed;
        }

        public HalloweenEventBag(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF5F1F>Special Event Reward Bag</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Double-click to unwrap spooky treasures!</BASEFONT>");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.PlaySound(0x5B4);
            from.FixedParticles(0x376A, 9, 32, 5030, 1161, 0, EffectLayer.Waist);

            // Always grant Halloween Pumpkins & Pies
            from.AddToBackpack(new HalloweenEventPumpkin(Utility.RandomMinMax(5, 15)));
            from.AddToBackpack(new BlackberryPumpkinPie() { Amount = Utility.RandomMinMax(2, 5) });

            // Random bonus roll
            switch (Utility.Random(10))
            {
                case 0:
                    from.AddToBackpack(new MaskOfTheJackOLantern());
                    from.SendMessage(0x35, "You received a Mask of the Jack-o'-Lantern!");
                    break;
                case 1:
                    from.AddToBackpack(new SpookySpiderwebSash());
                    from.SendMessage(0x35, "You received a Spooky Spiderweb Sash!");
                    break;
                case 2:
                case 3:
                case 4:
                    Item dye = HalloweenCemeteryEvent.CreateRandomHalloweenDye();
                    if (dye != null)
                    {
                        from.AddToBackpack(dye);
                        from.SendMessage(0x35, "You received a rare {0}!", dye.Name);
                    }
                    break;
                default:
                    from.SendMessage(0x35, "You unwrap Halloween Pumpkins and Blackberry Pumpkin Pies!");
                    break;
            }

            Delete();
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
