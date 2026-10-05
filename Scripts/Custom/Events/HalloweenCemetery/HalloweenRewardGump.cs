using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public enum SlayerSelectTarget
    {
        CompositeBow,
        SoulGlaive,
        Spellbook
    }

    public class HalloweenRewardEntry
    {
        public Type ItemType { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int ItemID { get; set; }
        public int Hue { get; set; }
        public string Description { get; set; }
        public bool IsSlayerChooser { get; set; }
        public SlayerSelectTarget SlayerTarget { get; set; }

        public HalloweenRewardEntry(Type itemType, string name, int cost, int itemID, int hue, string description)
        {
            ItemType = itemType;
            Name = name;
            Cost = cost;
            ItemID = itemID;
            Hue = hue;
            Description = description;
            IsSlayerChooser = false;
        }

        public HalloweenRewardEntry(SlayerSelectTarget target, string name, int cost, int itemID, int hue, string description)
        {
            SlayerTarget = target;
            Name = name;
            Cost = cost;
            ItemID = itemID;
            Hue = hue;
            Description = description;
            IsSlayerChooser = true;
        }
    }

    public class HalloweenRewardGump : Gump
    {
        private readonly Mobile m_From;
        private readonly int m_Tab;

        private const int GumpWidth = 640;
        private const int GumpHeight = 540;

        public static readonly List<HalloweenRewardEntry> DyesRewards = new List<HalloweenRewardEntry>
        {
            new HalloweenRewardEntry(typeof(CandyCornOrangeHairDye), "Candy Corn Orange Hair Dye", 150, 0xEFE, 1161, "Permanent Candy Corn Orange hair dye."),
            new HalloweenRewardEntry(typeof(CandyCornOrangeBeardDye), "Candy Corn Orange Beard Dye", 150, 0xEFE, 1161, "Permanent Candy Corn Orange facial hair dye."),
            new HalloweenRewardEntry(typeof(SpiderSilkWhiteHairDye), "Spider Silk White Hair Dye", 150, 0xEFE, 1153, "Permanent Spider Silk White hair dye."),
            new HalloweenRewardEntry(typeof(SpiderSilkWhiteBeardDye), "Spider Silk White Beard Dye", 150, 0xEFE, 1153, "Permanent Spider Silk White facial hair dye."),
            new HalloweenRewardEntry(typeof(VibrantCrimsonHairDye), "Vibrant Crimson Hair Dye", 150, 0xEFE, 1964, "Permanent Vibrant Crimson hair dye."),
            new HalloweenRewardEntry(typeof(VibrantCrimsonBeardDye), "Vibrant Crimson Beard Dye", 150, 0xEFE, 1964, "Permanent Vibrant Crimson facial hair dye."),
            new HalloweenRewardEntry(typeof(SpectralVenomHairDye), "Spectral Venom Hair Dye", 150, 0xEFE, 1655, "Permanent caustic Spectral Venom hair dye."),
            new HalloweenRewardEntry(typeof(SpectralVenomBeardDye), "Spectral Venom Beard Dye", 150, 0xEFE, 1655, "Permanent caustic Spectral Venom facial hair dye."),
            new HalloweenRewardEntry(typeof(SpectralAmethystHairDye), "Spectral Amethyst Hair Dye", 150, 0xEFE, 1296, "Permanent royal Spectral Amethyst hair dye."),
            new HalloweenRewardEntry(typeof(SpectralAmethystBeardDye), "Spectral Amethyst Beard Dye", 150, 0xEFE, 1296, "Permanent royal Spectral Amethyst facial hair dye.")
        };

        public static readonly List<HalloweenRewardEntry> SlayerWeaponsRewards = new List<HalloweenRewardEntry>
        {
            new HalloweenRewardEntry(SlayerSelectTarget.CompositeBow, "Hellspire Composite Bow (Pick Slayer)", 900, 0x26C2, 1358, "70% Lightning, 50% Mana Leech, 60% HLA/HLD, 60 Velocity, SC, FC 1, 40% SSI, 60% DI. Elemental dmg & area match chosen slayer!"),
            new HalloweenRewardEntry(SlayerSelectTarget.SoulGlaive, "Hellspire Soul Glaive (Pick Slayer)", 900, 0x090A, 1358, "Thrower: 70% Lightning, 50% Mana Leech, 60% HLA/HLD, 60 Velocity, SC, FC 1, 40% SSI, 60% DI. Elemental dmg & area match chosen slayer!"),
            new HalloweenRewardEntry(SlayerSelectTarget.Spellbook, "Grimoire of the Crypt (Pick Slayer)", 900, 0xEFA, 1161, "Full 64 Magery Spells, 50% Spell Damage, FC 1, FCR 1, MR 2, with your chosen Slayer!")
        };

        public static readonly List<HalloweenRewardEntry> EquipmentRewards = new List<HalloweenRewardEntry>
        {
            new HalloweenRewardEntry(typeof(HalloweenDragonBondedStatuette), "Halloween Dragon Statuette (Bonded)", 10000, 0x20D6, 1161, "Requires GM Taming & Lore. 3-slot trainable pet dragon with Cu Sidhe healing, Blaze Orange."),
            new HalloweenRewardEntry(typeof(MaskOfTheJackOLantern), "Mask of the Jack-o'-Lantern", 450, 0xA3EA, 1161, "Cosmetic Jack-o'-Lantern transmog helmet with no stats or requirements."),
            new HalloweenRewardEntry(typeof(SpookySpiderwebSash), "Spooky Spiderweb Sash", 450, 0x1541, 1150, "Spectral sash with DCI 10%, HCI 5%, +10 Stam, FC 1, LMC 4%."),
            new HalloweenRewardEntry(typeof(GraveRobbersHaversack), "Grave Robber's Haversack", 525, 0x9B2, 1168, "Witch-green backpack with 50% Weight Reduction."),
            new HalloweenRewardEntry(typeof(JacksSoulLantern), "Jack's Soul Lantern", 600, 0x2F5B, 1161, "Undead slayer talisman, Night Sight, DCI 10%, SDI 12%, LMC 8%, FCR 1."),
            new HalloweenRewardEntry(typeof(MantleOfTheCryptLord), "Mantle of the Crypt Lord", 750, 0x1515, 1170, "Blessed cloak / wing armor: +8 Hits/Mana/Stam, MR 2, LRC 15%, All Resists +5%. Alterable for Gargoyles."),
            new HalloweenRewardEntry(typeof(ReapersHarvestScythe), "Reaper's Harvest Scythe", 900, 0x26BA, 1157, "Undead slayer scythe, SC, DI 50%, SSI 30%, HLL/HML/HL 50%, Soul Harvest."),
            new HalloweenRewardEntry(typeof(HalloweenEventBag), "Halloween Event Bag", 150, 0xE76, 1161, "Event prize bag filled with pumpkins, pies, and chances at rare artifacts and dyes.")
        };

        public static readonly List<HalloweenRewardEntry> HouseRewards = new List<HalloweenRewardEntry>
        {
            new HalloweenRewardEntry(typeof(WitchesCauldronDeed), "Witches' Brew Cauldron Deed", 1050, 0x14F0, 1168, "Interactive house addon dispensing Halloween sweets once per hour."),
            new HalloweenRewardEntry(typeof(TombstoneOfTheAwakenedDeadDeed), "Tombstone of Awakened Dead", 1050, 0x14F0, 1150, "Interactive gravestone house addon with spectral apparitions.")
        };

        public HalloweenRewardGump(Mobile from, int tab = 0) : base(50, 50)
        {
            m_From = from;
            m_Tab = Math.Max(0, Math.Min(3, tab));

            int pumpkins = CountPumpkins(from);

            List<HalloweenRewardEntry> currentList;
            switch (m_Tab)
            {
                case 0: currentList = DyesRewards; break;
                case 1: currentList = SlayerWeaponsRewards; break;
                case 2: currentList = EquipmentRewards; break;
                default: currentList = HouseRewards; break;
            }

            int startY = 108;
            int itemH = (m_Tab == 1) ? 62 : (m_Tab == 0 ? 40 : 50);
            int gumpHeight = Math.Max(260, startY + (currentList.Count * itemH) + 20);

            AddPage(0);
            AddBackground(0, 0, GumpWidth, gumpHeight, 9270);
            AddBackground(15, 15, GumpWidth - 30, 55, 9390);

            // Title
            AddHtml(25, 20, GumpWidth - 50, 25, "<BASEFONT COLOR=#FF7700 size=6><CENTER><b>Halloween Cemetery Event Rewards</b></CENTER></BASEFONT>", false, false);
            AddHtml(25, 45, GumpWidth - 50, 20, String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Your Halloween Pumpkins: <BASEFONT COLOR=#FFA500><b>{0}</b></BASEFONT></CENTER></BASEFONT>", pumpkins), false, false);

            // Navigation Tabs
            int tabY = 75;
            int btnWidth = 105;
            int htmlWidth = 125;
            string[] tabNames = { "Special Dyes", "Slayer Armory", "Exclusive Loot", "House" };
            int[] tabX = { 51, 195, 339, 483 };

            for (int i = 0; i < tabNames.Length; i++)
            {
                int x = tabX[i];
                bool active = (m_Tab == i);

                AddButton(x, tabY, 0x98D, 0x98D, 1 + i, GumpButtonType.Reply, 0);

                string color = active ? "#FFA500" : "#FFFFFF";
                int htmlX = x - ((htmlWidth - btnWidth) / 2);
                AddHtml(htmlX, tabY + 2, htmlWidth, 20, String.Format("<BASEFONT COLOR={0}><CENTER><b>{1}</b></CENTER></BASEFONT>", color, tabNames[i]), false, false);
            }

            for (int i = 0; i < currentList.Count; i++)
            {
                HalloweenRewardEntry entry = currentList[i];
                int entryY = startY + (i * itemH);
                bool canAfford = pumpkins >= entry.Cost;

                // Entry background row
                AddBackground(15, entryY, GumpWidth - 30, itemH - 2, 9390);

                // Item art preview (offset to x=35 to avoid clipping into left scroll curl)
                AddItem(35, entryY + (itemH > 40 ? 8 : 4), entry.ItemID, entry.Hue);

                // Name and description (dark high-contrast text on light parchment)
                AddHtml(85, entryY + 2, 355, 18, String.Format("<BASEFONT COLOR=#FF7700><b>{0}</b></BASEFONT>", entry.Name), false, false);
                AddHtml(85, entryY + 20, 355, (itemH > 40 ? itemH - 22 : 18), String.Format("<BASEFONT COLOR=#222222 size=1>{0}</BASEFONT>", entry.Description), false, false);

                // Cost label
                AddHtml(445, entryY + (itemH / 2 - 10), 120, 20, String.Format("<BASEFONT COLOR={0}><b>{1} Pumpkins</b></BASEFONT>", canAfford ? "#005500" : "#B22222", entry.Cost), false, false);

                // Purchase button
                AddButton(570, entryY + (itemH / 2 - 12), 0xFB7, 0xFB9, 100 + i, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: return; // Close
                case 1: from.SendGump(new HalloweenRewardGump(from, 0)); return;
                case 2: from.SendGump(new HalloweenRewardGump(from, 1)); return;
                case 3: from.SendGump(new HalloweenRewardGump(from, 2)); return;
                case 4: from.SendGump(new HalloweenRewardGump(from, 3)); return;
            }

            int index = info.ButtonID - 100;
            List<HalloweenRewardEntry> currentList;
            switch (m_Tab)
            {
                case 0: currentList = DyesRewards; break;
                case 1: currentList = SlayerWeaponsRewards; break;
                case 2: currentList = EquipmentRewards; break;
                default: currentList = HouseRewards; break;
            }

            if (index < 0 || index >= currentList.Count)
                return;

            HalloweenRewardEntry selected = currentList[index];

            if (from.Backpack == null)
                return;

            int pumpkins = CountPumpkins(from);
            if (pumpkins < selected.Cost)
            {
                from.SendMessage(0x22, "You need {0} Halloween Pumpkins for that, but only have {1}.", selected.Cost, pumpkins);
                from.SendGump(new HalloweenRewardGump(from, m_Tab));
                return;
            }

            // Animal Taming & Animal Lore skill requirement for Halloween Dragon
            if (selected.ItemType == typeof(HalloweenDragonBondedStatuette))
            {
                if (from.Skills[SkillName.AnimalTaming].Value < 100.0 || from.Skills[SkillName.AnimalLore].Value < 100.0)
                {
                    from.SendMessage(0x22, "You must have at least 100.0 Animal Taming and 100.0 Animal Lore to claim this pet.");
                    from.SendGump(new HalloweenRewardGump(from, m_Tab));
                    return;
                }
            }

            // If it's a Slayer choice weapon/spellbook, open Slayer Selection Gump
            if (selected.IsSlayerChooser)
            {
                from.CloseGump(typeof(HalloweenSlayerSelectGump));
                from.SendGump(new HalloweenSlayerSelectGump(from, selected.SlayerTarget, selected.Cost, m_Tab));
                return;
            }

            // Standard direct purchase
            if (!from.Backpack.ConsumeTotal(typeof(HalloweenEventPumpkin), selected.Cost))
            {
                from.SendMessage(0x22, "Could not consume the required Halloween Pumpkins.");
                from.SendGump(new HalloweenRewardGump(from, m_Tab));
                return;
            }

            Item reward = null;
            try
            {
                if (selected.ItemType == typeof(MantleOfTheCryptLord) && from.Race == Race.Gargoyle)
                {
                    reward = new GargishMantleOfTheCryptLord();
                }
                else
                {
                    reward = Activator.CreateInstance(selected.ItemType) as Item;
                }
            }
            catch
            {
            }

            if (reward == null)
            {
                from.AddToBackpack(new HalloweenEventPumpkin(selected.Cost)); // refund
                from.SendMessage(0x22, "An error occurred creating the reward item. Your pumpkins have been refunded.");
                return;
            }

            from.AddToBackpack(reward);
            from.PlaySound(0x5B4);
            from.FixedParticles(0x376A, 9, 32, 5030, 1161, 0, EffectLayer.Waist);
            from.SendMessage(0x35, "Congratulations! You have redeemed {0} Halloween Pumpkins for {1}!", selected.Cost, selected.Name);

            from.SendGump(new HalloweenRewardGump(from, m_Tab));
        }

        public static int CountPumpkins(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return 0;

            return from.Backpack.GetAmount(typeof(HalloweenEventPumpkin));
        }
    }

    #region Slayer Selection Gump
    public class HalloweenSlayerSelectGump : Gump
    {
        private readonly Mobile m_From;
        private readonly SlayerSelectTarget m_Target;
        private readonly int m_Cost;
        private readonly int m_ReturnTab;

        private static readonly SlayerName[] m_Slayers = new SlayerName[]
        {
            SlayerName.Silver,          // Undead
            SlayerName.Exorcism,        // Demon
            SlayerName.ArachnidDoom,    // Arachnid
            SlayerName.ReptilianDeath,  // Reptile
            SlayerName.ElementalBan,    // Elemental
            SlayerName.Repond           // Repond
        };

        public HalloweenSlayerSelectGump(Mobile from, SlayerSelectTarget target, int cost, int returnTab) : base(120, 100)
        {
            m_From = from;
            m_Target = target;
            m_Cost = cost;
            m_ReturnTab = returnTab;

            string itemTitle = target == SlayerSelectTarget.CompositeBow ? "Hellspire Composite Bow" :
                               target == SlayerSelectTarget.SoulGlaive ? "Hellspire Soul Glaive" : "Grimoire of the Crypt (64 Spells)";

            int artID = target == SlayerSelectTarget.CompositeBow ? 0x26C2 :
                        target == SlayerSelectTarget.SoulGlaive ? 0x090A : 0xEFA;

            AddPage(0);
            AddBackground(0, 0, 420, 360, 9270);
            AddBackground(15, 15, 390, 50, 9390);

            AddItem(25, 20, artID, 1358);
            AddHtml(75, 20, 320, 20, String.Format("<BASEFONT COLOR=#FF7700 size=4><b>{0}</b></BASEFONT>", itemTitle), false, false);

            int pumpkins = HalloweenRewardGump.CountPumpkins(from);
            bool canAfford = pumpkins >= cost;
            string costColor = canAfford ? "#005500" : "#B22222";
            AddHtml(75, 40, 320, 20, String.Format("<BASEFONT COLOR=#222222>Cost: <BASEFONT COLOR={0}><b>{1} Pumpkins</b></BASEFONT> (You have: <b>{2}</b>)</BASEFONT>", costColor, cost, pumpkins), false, false);

            AddHtml(25, 75, 370, 35, "<BASEFONT COLOR=#C0C0C0>Choose which Slayer power you wish to imbue into your new equipment:</BASEFONT>", false, false);

            int startY = 115;
            for (int i = 0; i < m_Slayers.Length; i++)
            {
                int rowY = startY + (i * 32);
                AddBackground(20, rowY, 380, 28, 9390);

                AddButton(28, rowY + 3, 0xFA5, 0xFA7, 10 + i, GumpButtonType.Reply, 0);
                AddHtml(65, rowY + 4, 120, 20, String.Format("<BASEFONT COLOR=#FFA500><b>{0} Slayer</b></BASEFONT>", HalloweenSlayerHelper.GetSlayerTitle(m_Slayers[i])), false, false);

                if (target != SlayerSelectTarget.Spellbook)
                {
                    AddHtml(185, rowY + 4, 210, 20, String.Format("<BASEFONT COLOR=#222222 size=1>({0})</BASEFONT>", HalloweenSlayerHelper.GetElementalSummary(m_Slayers[i])), false, false);
                }
            }

            AddButton(180, 318, 0x995, 0x996, 0, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
            {
                from.SendGump(new HalloweenRewardGump(from, m_ReturnTab));
                return;
            }

            int index = info.ButtonID - 10;
            if (index < 0 || index >= m_Slayers.Length)
                return;

            SlayerName chosenSlayer = m_Slayers[index];

            if (from.Backpack == null)
                return;

            int pumpkins = HalloweenRewardGump.CountPumpkins(from);
            if (pumpkins < m_Cost)
            {
                from.SendMessage(0x22, "You do not have enough Halloween Pumpkins ({0} required).", m_Cost);
                from.SendGump(new HalloweenRewardGump(from, m_ReturnTab));
                return;
            }

            if (!from.Backpack.ConsumeTotal(typeof(HalloweenEventPumpkin), m_Cost))
            {
                from.SendMessage(0x22, "Could not consume the required Halloween Pumpkins.");
                from.SendGump(new HalloweenRewardGump(from, m_ReturnTab));
                return;
            }

            Item createdItem = null;
            switch (m_Target)
            {
                case SlayerSelectTarget.CompositeBow:
                    createdItem = new HalloweenCompositeBow(chosenSlayer);
                    break;
                case SlayerSelectTarget.SoulGlaive:
                    createdItem = new HalloweenSoulGlaive(chosenSlayer);
                    break;
                case SlayerSelectTarget.Spellbook:
                    createdItem = new HalloweenSpellbook(chosenSlayer);
                    break;
            }

            if (createdItem == null)
            {
                from.AddToBackpack(new HalloweenEventPumpkin(m_Cost)); // refund
                from.SendMessage(0x22, "An error occurred creating your item. Pumpkins refunded.");
                return;
            }

            from.AddToBackpack(createdItem);
            from.PlaySound(0x5B4);
            from.FixedParticles(0x376A, 9, 32, 5030, 1161, 0, EffectLayer.Waist);
            from.SendMessage(0x35, "Congratulations! You have claimed the {0}!", createdItem.Name);

            from.SendGump(new HalloweenRewardGump(from, m_ReturnTab));
        }
    }
    #endregion
}
