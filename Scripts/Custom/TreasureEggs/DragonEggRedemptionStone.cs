using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [Flipable(0xED4, 0xED5)]
    public class DragonEggRedemptionStone : Item
    {
        public const int EggCost = 500;

        public Dictionary<Mobile, int> Deposits { get; private set; }

        [Constructable]
        public DragonEggRedemptionStone()
            : base(0xED4)
        {
            Name = "a Dragon Egg Redemption Stone";
            Hue = 1152;
            Movable = false;

            Deposits = new Dictionary<Mobile, int>();
        }

        public DragonEggRedemptionStone(Serial serial)
            : base(serial)
        {
            Deposits = new Dictionary<Mobile, int>();
        }

        public int GetDeposit(Mobile m)
        {
            int count;
            Deposits.TryGetValue(m, out count);
            return count;
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

        public int DepositAll(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            var eggs = m.Backpack.FindItemsByType<DragonEgg>(true);
            int total = eggs.Sum(e => e.Amount);

            if (total <= 0)
                return 0;

            foreach (var egg in eggs)
                egg.Delete();

            int current;
            Deposits.TryGetValue(m, out current);
            Deposits[m] = current + total;

            return total;
        }

        public bool Redeem(Mobile m, int amount)
        {
            int current;
            Deposits.TryGetValue(m, out current);

            if (current < amount)
                return false;

            Deposits[m] = current - amount;
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Deposits.Count);
            foreach (var kvp in Deposits)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                int amt = reader.ReadInt();

                if (m != null)
                    Deposits[m] = amt;
            }
        }
    }

    public class DragonEggRedemptionGump : Gump
    {
        private readonly DragonEggRedemptionStone m_Stone;

        public DragonEggRedemptionGump(PlayerMobile pm, DragonEggRedemptionStone stone)
            : base(100, 100)
        {
            m_Stone = stone;

            int banked = stone.GetDeposit(pm);
            int inPack = pm.Backpack == null ? 0
                : pm.Backpack.FindItemsByType<DragonEgg>(true).Sum(e => e.Amount);

            Closable = true;
            Disposable = true;
            Dragable = true;

            // Bumped from 430 -> 470 height when the 6th statuette (Flame Of
            // Abrahel) was added, so the close button at y=410 has breathing
            // room and the trailing labels don't bump the frame.
            AddBackground(0, 0, 440, 470, 5054);
            AddBackground(10, 10, 420, 450, 3500);

            AddLabel(120, 22, 53, "Dragon Egg Redemption");

            AddLabel(25, 60, 0, string.Format("Banked on stone: {0}", banked));
            AddLabel(25, 80, 0, string.Format("In your backpack: {0}", inPack));
            AddLabel(25, 100, 0x22, string.Format("Cost per statuette: {0} eggs.", DragonEggRedemptionStone.EggCost));

            AddButton(25, 130, 4005, 4007, 100, GumpButtonType.Reply, 0);
            AddLabel(60, 130, inPack > 0 ? (ushort)68 : (ushort)0x22,
                inPack > 0
                    ? string.Format("Deposit {0} eggs from your pack.", inPack)
                    : "Deposit (no eggs in pack).");

            AddLabel(25, 170, 53, "Redeem a statuette:");

            AddButton(30, 200, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(65, 200, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Hellfire Steed");

            AddButton(30, 225, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(65, 225, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Bane Dragon");

            AddButton(30, 250, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(65, 250, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Wildfire Ostard");

            AddButton(30, 275, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(65, 275, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Dragon Hildebrandt");

            AddButton(30, 300, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddLabel(65, 300, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Ozymandias' Hiryu");

            AddButton(30, 325, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddLabel(65, 325, banked >= DragonEggRedemptionStone.EggCost ? (ushort)68 : (ushort)0x22,
                "Flame Of Abrahel");

            AddLabel(25, 360, 0x22, "Statuettes pop a pet when double-clicked.");
            AddLabel(25, 380, 0x22, "Summoned pets have a small chance for a rare color.");

            AddButton(360, 430, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddLabel(320, 430, 0, "Close");
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

            if (info.ButtonID == 100)
            {
                int deposited = m_Stone.DepositAll(from);

                if (deposited > 0)
                    from.SendMessage(string.Format("You deposit {0} dragon eggs. Total banked: {1}.",
                        deposited, m_Stone.GetDeposit(from)));
                else
                    from.SendMessage("You have no dragon eggs in your backpack to deposit.");

                pm.SendGump(new DragonEggRedemptionGump(pm, m_Stone));
                return;
            }

            Item reward = null;
            switch (info.ButtonID)
            {
                case 1: reward = new HellfireSteedBondedStatuette(); break;
                case 2: reward = new BaneDragonBondedStatuette(); break;
                case 3: reward = new WildfireOstardBondedStatuette(); break;
                case 4: reward = new DragonHildebrandtBondedStatuette(); break;
                case 5: reward = new OzymandiasHiryuBondedStatuette(); break;
                case 6: reward = new FlameOfAbrahelBondedStatuette(); break;
            }

            if (reward == null)
                return;

            if (m_Stone.GetDeposit(from) < DragonEggRedemptionStone.EggCost)
            {
                from.SendMessage(string.Format("You need {0} banked eggs to redeem. You have {1}.",
                    DragonEggRedemptionStone.EggCost, m_Stone.GetDeposit(from)));
                reward.Delete();
                pm.SendGump(new DragonEggRedemptionGump(pm, m_Stone));
                return;
            }

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, reward, false))
            {
                from.SendMessage("Your backpack cannot hold the statuette. Make room and try again.");
                reward.Delete();
                return;
            }

            m_Stone.Redeem(from, DragonEggRedemptionStone.EggCost);

            from.SendMessage(string.Format("You redeem 1000 dragon eggs for a statuette. Remaining: {0}.",
                m_Stone.GetDeposit(from)));
            from.PlaySound(0x1EA);

            pm.SendGump(new DragonEggRedemptionGump(pm, m_Stone));
        }
    }
}
