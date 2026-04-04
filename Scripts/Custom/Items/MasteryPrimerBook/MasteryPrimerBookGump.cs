/*
Author: Cork
Creation Date: 3/22/2026
Description:
    Volumes page — lists Volume I, II, III with indicators for which have primers
    Skills page — shows all 19 mastery skills split across two columns, with indicators for available primers
    Skill detail page — shows all 3 volumes for the selected skill with counts and retrieve buttons
*/
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class MasteryPrimerBookGump : BaseGump
    {
        public MasteryPrimerBook Book { get; private set; }

        public SkillCat Category { get; set; }
        public SkillName Skill { get; set; }
        public bool SkillSelected { get; set; }

        public MasteryPrimerBookGump(PlayerMobile pm, MasteryPrimerBook book)
            : base(pm, 150, 200)
        {
            Book = book;

            pm.CloseGump(typeof(MasteryPrimerBookGump));
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

        public void BuildCategoriesPage()
        {
            AddHtml(0, 25, 175, 20, Center("Mastery Primers"), false, false);

            if (Book == null || Book.Deleted || MasteryPrimerBook._SkillInfo == null)
                return;

            int index = 0;
            foreach (var kvp in MasteryPrimerBook._SkillInfo)
            {
                AddHtmlLocalized(45, 55 + (index * 15), 100, 20, BaseSpecialScrollBook.GetCategoryLocalization(kvp.Key), false, false);

                if (Book.HasPrimerForCategory(kvp.Value))
                {
                    AddButton(30, 59 + (index * 15), 2103, 2104, 10 + (int)kvp.Key, GumpButtonType.Reply, 0);
                }

                index++;
            }
        }

        public void BuildSkillsPage()
        {
            AddHtmlLocalized(0, 25, 175, 20, CenterLoc, String.Format("#{0}", BaseSpecialScrollBook.GetCategoryLocalization(Category)), 0, false, false);

            if (Book == null || Book.Deleted || MasteryPrimerBook._SkillInfo == null || !MasteryPrimerBook._SkillInfo.ContainsKey(Category))
                return;

            List<SkillName> list = MasteryPrimerBook._SkillInfo[Category];

            int x = 45;
            int y = 55;
            int buttonX = 30;
            int split = list.Count >= 9 ? list.Count / 2 : -1;

            for (int i = 0; i < list.Count; i++)
            {
                SkillName skill = list[i];

                if (split > -1 && i == split)
                {
                    x = 205;
                    y = 55;
                    buttonX = 190;
                }

                AddHtmlLocalized(x, y, 110, 20, SkillInfo.Table[(int)skill].Localization, false, false);

                if (Book.HasPrimerForSkill(skill))
                {
                    AddButton(buttonX, y + 4, 2103, 2104, 100 + i, GumpButtonType.Reply, 0);
                }

                y += 15;
            }
        }

        public void BuildSkillPage()
        {
            AddHtmlLocalized(0, 25, 175, 20, CenterLoc, String.Format("#{0}", SkillInfo.Table[(int)Skill].Localization), 0, false, false);

            if (Book == null || Book.Deleted)
                return;

            int y = 55;

            for (int i = 0; i < MasteryPrimerBook.Volumes.Length; i++)
            {
                int volume = MasteryPrimerBook.Volumes[i];
                int count = Book.GetPrimerCount(Skill, volume);

                AddHtml(45, y, 65, 20, MasteryPrimerBook.VolumeNames[i], false, false);
                AddLabel(140, y, 0, count.ToString());

                if (count > 0)
                {
                    AddButton(30, y + 4, 2437, 2438, 1000 + volume, GumpButtonType.Reply, 0);
                }

                y += 15;
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 1)
            {
                // Back to skills page
                SkillSelected = false;
                Refresh();
            }
            else if (id == 2)
            {
                // Back to categories page
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

                if (Category != SkillCat.None && MasteryPrimerBook._SkillInfo.ContainsKey(Category))
                {
                    List<SkillName> list = MasteryPrimerBook._SkillInfo[Category];

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
                // Retrieve primer
                int volume = id - 1000;

                Book.ConstructPrimer(User, Skill, volume);

                Refresh();
            }
        }
    }
}
