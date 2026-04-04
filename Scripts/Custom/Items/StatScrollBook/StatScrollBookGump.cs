/*
Author: Cork
Creation Date: 3/22/2026
Description:
    Uses the same book background as the other scroll book gumps
    Displays 5 sections: +5, +10, +15, +20, +25 stat scrolls
    Shows the count of scrolls at each tier
    Provides retrieve buttons when scrolls are available
    Refreshes after retrieving a scroll
    The book is [Constructable] so you can create it in-game with [add StatScrollBook.
*/
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;

namespace Server.Gumps
{
    public class StatScrollBookGump : BaseGump
    {
        public StatScrollBook Book { get; private set; }

        public StatScrollBookGump(PlayerMobile pm, StatScrollBook book)
            : base(pm, 150, 200)
        {
            Book = book;

            pm.CloseGump(typeof(StatScrollBookGump));
        }

        private static readonly string[] ScrollNames = new string[] { "Wondrous", "Exalted", "Mythical", "Legendary", "Ultimate" };
        private static readonly int[] ScrollBonuses = new int[] { 5, 10, 15, 20, 25 };

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 2200);

            for (int i = 0; i < 2; ++i)
            {
                int xOffset = 25 + (i * 165);

                AddImage(xOffset, 45, 57);
                xOffset += 20;

                for (int j = 0; j < 6; ++j, xOffset += 15)
                    AddImage(xOffset, 45, 58);

                AddImage(xOffset - 5, 45, 59);
            }

            AddHtml(0, 15, 175, 20, Center("Stat Scrolls"), false, false);

            if (Book == null || Book.Deleted)
                return;

            int y = 55;

            for (int i = 0; i < ScrollBonuses.Length; i++)
            {
                int bonus = ScrollBonuses[i];
                int count = Book.GetScrollCount(bonus);

                AddHtml(45, y, 65, 20, ScrollNames[i], false, false);
                AddLabel(110, y, 0, String.Format("({0})", bonus));
                AddLabel(140, y, 0, count.ToString());

                if (count > 0)
                {
                    AddButton(30, y + 4, 2103, 2104, 100 + i, GumpButtonType.Reply, 0);
                }

                y += 15;
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id >= 100 && id < 100 + ScrollBonuses.Length)
            {
                int index = id - 100;
                int bonus = ScrollBonuses[index];
                double value = StatScrollBook.StatCap + bonus;

                Book.ConstructStatScroll(User, value);

                Refresh();
            }
        }
    }
}
