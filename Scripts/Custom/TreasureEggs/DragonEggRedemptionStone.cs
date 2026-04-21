using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Linq;

namespace Server.Items
{
    [Flipable(0xED4, 0xED5)]
    public class DragonEggRedemptionStone : Item
    {
        public const int EggCost = 1000;

        [Constructable]
        public DragonEggRedemptionStone()
            : base(0xED4)
        {
            Name = "a Dragon Egg Redemption Stone";
            Hue = 1175;
            Movable = false;
        }

        public DragonEggRedemptionStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (from is PlayerMobile pm)
            {
                pm.CloseGump(typeof(DragonEggRedemptionGump));
                pm.SendGump(new DragonEggRedemptionGump(pm, this));
            }
        }

        public static int CountEggs(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            return m.Backpack.FindItemsByType<DragonEgg>(true).Sum(e => e.Amount);
        }

        public static bool ConsumeEggs(Mobile m, int amount)
        {
            if (m.Backpack == null)
                return false;

            int remaining = amount;
            var eggs = m.Backpack.FindItemsByType<DragonEgg>(true);

            if (eggs.Sum(e => e.Amount) < amount)
                return false;

            foreach (var egg in eggs)
            {
                if (remaining <= 0)
                    break;

                if (egg.Amount <= remaining)
                {
                    remaining -= egg.Amount;
                    egg.Delete();
                }
                else
                {
                    egg.Amount -= remaining;
                    remaining = 0;
                }
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DragonEggRedemptionGump : Gump
    {
        private readonly DragonEggRedemptionStone m_Stone;

        public DragonEggRedemptionGump(PlayerMobile pm, DragonEggRedemptionStone stone)
            : base(100, 100)
        {
            m_Stone = stone;

            int eggs = DragonEggRedemptionStone.CountEggs(pm);

            Closable = true;
            Disposable = true;
            Dragable = true;

            AddBackground(0, 0, 420, 340, 5054);
            AddBackground(10, 10, 400, 320, 3500);

            AddLabel(120, 22, 53, "Dragon Egg Redemption");
            AddLabel(25, 60, 0, string.Format("You have {0} dragon eggs. Each reward costs {1}.", eggs, DragonEggRedemptionStone.EggCost));

            AddLabel(25, 95, 53, "Choose a statuette:");

            AddButton(30, 125, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(65, 125, 0, "Hellfire Steed");

            AddButton(30, 155, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(65, 155, 0, "Bane Dragon");

            AddButton(30, 185, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(65, 185, 0, "Wildfire Ostard");

            AddButton(30, 215, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(65, 215, 0, "Dragon Hildebrandt");

            AddLabel(25, 260, 0x22, "Double-click a statuette to summon.");
            AddLabel(25, 280, 0x22, "Summoned pets have a small chance for a rare color.");

            AddButton(340, 295, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddLabel(300, 295, 0, "Cancel");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || !(from is PlayerMobile pm))
                return;

            if (!from.InRange(m_Stone.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (DragonEggRedemptionStone.CountEggs(from) < DragonEggRedemptionStone.EggCost)
            {
                from.SendMessage(string.Format("You need {0} dragon eggs to redeem a reward.", DragonEggRedemptionStone.EggCost));
                return;
            }

            Item reward = null;

            switch (info.ButtonID)
            {
                case 1: reward = new HellfireSteedBondedStatuette(); break;
                case 2: reward = new BaneDragonBondedStatuette(); break;
                case 3: reward = new WildfireOstardBondedStatuette(); break;
                case 4: reward = new DragonHildebrandtBondedStatuette(); break;
            }

            if (reward == null)
                return;

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, reward, false))
            {
                from.SendMessage("Your backpack cannot hold the statuette. Make room and try again.");
                reward.Delete();
                return;
            }

            DragonEggRedemptionStone.ConsumeEggs(from, DragonEggRedemptionStone.EggCost);

            from.SendMessage("You redeem your dragon eggs for a bonded statuette.");
            from.PlaySound(0x1EA);
        }
    }
}
