using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{
    public class ScrollOfTranscendenceBookGump : SpecialScrollBookGump
    {
        // Page 1 (left): 0.1 – 0.7
        private static readonly double[] Page1Values = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 };

        // Page 2 (right): 0.8, 0.9, 1.0, 2.0, 3.0, 5.0
        private static readonly double[] Page2Values = { 0.8, 0.9, 1.0, 2.0, 3.0, 5.0 };

        public bool SkillSelected { get; set; }

        public ScrollOfTranscendenceBookGump(PlayerMobile pm, ScrollOfTranscendenceBook book)
            : base(pm, book)
        {
        }

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

            if (Category != SkillCat.None)
            {
                if (SkillSelected)
                {
                    BuildSkillPage();

                    AddButton(20, 5, 2205, 2205, 1, GumpButtonType.Reply, 0);
                }
                else
                {
                    BuildSkillsPage();

                    AddButton(20, 5, 2205, 2205, 2, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                BuildCategoriesPage();
            }
        }

        public override void BuildCategoriesPage()
        {
            AddHtml(0, 10, 175, 40, "<center>Scrolls of<br>Transcendence</center>", false, false);

            if (Book == null || Book.Deleted || Book.SkillInfo == null)
                return;

            int index = 0;
            foreach (var kvp in Book.SkillInfo)
            {
                AddHtmlLocalized(45, 55 + (index * 15), 100, 20, BaseSpecialScrollBook.GetCategoryLocalization(kvp.Key), false, false);

                if (HasScroll(kvp.Value))
                {
                    AddButton(30, 59 + (index * 15), 2103, 2104, 10 + (int)kvp.Key, GumpButtonType.Reply, 0);
                }

                index++;
            }
        }

        public override void BuildSkillPage()
        {
            AddHtmlLocalized(0, 15, 175, 20, CenterLoc, String.Format("#{0}", SkillInfo.Table[(int)Skill].Localization), 0, false, false);

            if (Book == null || Book.Deleted)
                return;

            for (int i = 0; i < Page1Values.Length; i++)
            {
                double value = Page1Values[i];
                int total = GetTotalScrolls(Skill, value);
                int y = 55 + (i * 15);

                AddHtml(40, y, 150, 20, String.Format("<basefont size=2><b>{0:0.0} Skill Increase: {1}</b>", value, total), false, false);

                if (total > 0)
                    AddButton(30, y + 4, 2437, 2438, 1000 + (int)(value * 10), GumpButtonType.Reply, 0);
            }

            for (int i = 0; i < Page2Values.Length; i++)
            {
                double value = Page2Values[i];
                int total = GetTotalScrolls(Skill, value);
                int y = 55 + (i * 15);

                AddHtml(205, y, 150, 20, String.Format("<basefont size=2><b>{0:0.0} Skill Increase: {1}</b>", value, total), false, false);

                if (total > 0)
                    AddButton(195, y + 4, 2437, 2438, 1000 + (int)(value * 10), GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 1)
            {
                // Back to skills list
                SkillSelected = false;
                Refresh();
            }
            else if (id == 2)
            {
                // Back to categories
                Category = SkillCat.None;
                SkillSelected = false;
                Refresh();
            }
            else if (id >= 10 && id < 100)
            {
                // Category selected
                int cat = id - 10;

                if (cat >= 0 && cat <= 7)
                {
                    Category = (SkillCat)cat;
                    Refresh();
                }
            }
            else if (id >= 100 && id < 1000)
            {
                // Skill selected
                int index = id - 100;

                if (Category > SkillCat.None && Book.SkillInfo.ContainsKey(Category))
                {
                    List<SkillName> list = Book.SkillInfo[Category];

                    if (index >= 0 && index < list.Count)
                    {
                        Skill = list[index];
                        SkillSelected = true;
                        Refresh();
                    }
                }
            }
            else if (id >= 1000)
            {
                // Retrieve scroll
                double value = (id - 1000) / 10.0;

                Book.Construct(User, Skill, value);

                Refresh();
            }
        }
    }
}
