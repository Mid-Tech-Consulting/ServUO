using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Events.TurkeyEvent
{
    public enum TurkeySlayerTarget
    {
        DoubleAxe,
        PaladinHammer,
        Lajatang,
        GargishTalwar
    }

    public class TurkeyRewardEntry
    {
        public Type ItemType { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int ItemID { get; set; }
        public int Hue { get; set; }
        public string Description { get; set; }
        public bool IsSlayerChooser { get; set; }
        public TurkeySlayerTarget SlayerTarget { get; set; }
        public bool IsAkalabethStatue { get; set; }
        public bool IsQuiver { get; set; }
        public bool IsCincture { get; set; }

        public TurkeyRewardEntry(Type itemType, string name, int cost, int itemID, int hue, string description,
            bool isAkalabeth = false, bool isQuiver = false, bool isCincture = false)
        {
            ItemType = itemType;
            Name = name;
            Cost = cost;
            ItemID = itemID;
            Hue = hue;
            Description = description;
            IsAkalabethStatue = isAkalabeth;
            IsQuiver = isQuiver;
            IsCincture = isCincture;
            IsSlayerChooser = false;
        }

        public TurkeyRewardEntry(TurkeySlayerTarget target, string name, int cost, int itemID, int hue, string description)
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

    public class TurkeyRewardGump : Gump
    {
        private readonly Mobile m_From;

        private const int GumpWidth = 640;
        private const int GumpHeight = 580;

        // Exactly the 7 rewards - All on one single page (no tabs)
        public static readonly List<TurkeyRewardEntry> Rewards = new List<TurkeyRewardEntry>
        {
            // 1. Storm of Akalabeth Statuette
            new TurkeyRewardEntry(typeof(StormOfAkalabethStatue), "Storm of Akalabeth Statuette", 5000, 0xB1FE, 1373,
                "Redeemable mount (100% Energy dmg). 7 possible colors including 2 ultra-rares (1% chance). Requires GM Taming (100.0) to claim.", true),

            // 2. Riftbreaker Quiver (display graphic 0x2FB7 for beautiful upright quiver art)
            new TurkeyRewardEntry(typeof(RiftbreakerQuiver), "Riftbreaker Quiver", 1000, 0x2FB7, 0x0AD7,
                "Dex +8, Stam +15, Stam Regen 2, SSI 10%, HCI 10%, Luck 150, Archery/Throwing +5. (Gargoyles receive Gargish Riftbreaker Wing).", false, true, false),

            // 3. Kaelvok's Cincture (Graphic 0xA40E, Color 0x0AD0)
            new TurkeyRewardEntry(typeof(KaelvoksCincture), "Kaelvok's Cincture", 1000, 0xA40E, 0x0AD0,
                "Str +5, Hits +3, HCI 10%, SSI 10%, Tactics +10.0, Damage Eater 10%, Blessed. (Gargoyles receive Gargish Kaelvok's Cincture).", false, false, true),

            // 4. Autumn Harvest Double Axe
            new TurkeyRewardEntry(TurkeySlayerTarget.DoubleAxe, "Autumn Harvest Double Axe (Pick Slayer)", 1000, 0xF4B, TurkeySlayerHelper.WeaponHue,
                "Double Axe: 100% HML, 100% HLL, 50% HSL, 50% HLD, 70% Lightning, Brittle. Damage type & 50% Area match chosen slayer!"),

            // 5. Autumn Harvest Paladin Hammer
            new TurkeyRewardEntry(TurkeySlayerTarget.PaladinHammer, "Autumn Harvest Paladin Hammer (Pick Slayer)", 1000, 0x1439, TurkeySlayerHelper.WeaponHue,
                "War Hammer: 100% HML, 100% HLL, 50% HSL, 50% HLD, 70% Lightning, Brittle. Damage type & 50% Area match chosen slayer!"),

            // 6. Autumn Harvest Lajatang
            new TurkeyRewardEntry(TurkeySlayerTarget.Lajatang, "Autumn Harvest Lajatang (Pick Slayer)", 1000, 0x27A7, TurkeySlayerHelper.WeaponHue,
                "Lajatang: 100% HML, 100% HLL, 50% HSL, 50% HLD, 70% Lightning, Brittle. Damage type & 50% Area match chosen slayer!"),

            // 7. Autumn Harvest Gargish Talwar
            new TurkeyRewardEntry(TurkeySlayerTarget.GargishTalwar, "Autumn Harvest Gargish Talwar (Pick Slayer)", 1000, 0x908, TurkeySlayerHelper.WeaponHue,
                "Gargish Talwar: 100% HML, 100% HLL, 50% HSL, 50% HLD, 70% Lightning, Brittle. Damage type & 50% Area match chosen slayer!")
        };

        public TurkeyRewardGump(Mobile from, int tab = 0) : base(60, 40)
        {
            m_From = from;

            if (from == null)
                return;

            int feathers = CountFeathers(from);
            int packFeathers = CountBackpackFeathers(from);
            int bankFeathers = CountBankFeathers(from);

            AddPage(0);
            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);
            AddBackground(15, 15, GumpWidth - 30, 55, 9390);

            // Title
            AddHtml(25, 20, GumpWidth - 50, 25, "<BASEFONT COLOR=#FF7700 size=6><CENTER><b>Black Friday Turkey Rewards</b></CENTER></BASEFONT>", false, false);

            string featherText = bankFeathers > 0
                ? String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Your Turkey Feathers: <BASEFONT COLOR=#FFA500><b>{0:N0}</b></BASEFONT> ({1:N0} pack, {2:N0} bank)</CENTER></BASEFONT>", feathers, packFeathers, bankFeathers)
                : String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Your Turkey Feathers: <BASEFONT COLOR=#FFA500><b>{0:N0}</b></BASEFONT></CENTER></BASEFONT>", feathers);

            AddHtml(25, 45, GumpWidth - 50, 20, featherText, false, false);

            int startY = 78;
            int itemH = 62;

            // Single page showing all 7 rewards
            for (int i = 0; i < Rewards.Count; i++)
            {
                TurkeyRewardEntry entry = Rewards[i];
                int entryY = startY + (i * (itemH + 3));
                bool canAfford = feathers >= entry.Cost;

                // Entry background row
                AddBackground(15, entryY, GumpWidth - 30, itemH, 9390);

                // Preview Art (adjust art for Gargoyles on specific items)
                int itemArt = entry.ItemID;
                if (entry.IsQuiver && from.Race == Race.Gargoyle)
                    itemArt = 0x457E;
                else if (entry.IsCincture && from.Race == Race.Gargoyle)
                    itemArt = 0x50D8;

                // Item art positioning
                int artY = entry.IsQuiver ? entryY + 4 : entryY + 10;
                AddItem(35, artY, itemArt, entry.Hue);

                // Display Name
                string displayName = entry.Name;
                if (entry.IsQuiver && from.Race == Race.Gargoyle)
                    displayName = "Gargish Riftbreaker Wing";
                else if (entry.IsCincture && from.Race == Race.Gargoyle)
                    displayName = "Gargish Kaelvok's Cincture";

                AddHtml(85, entryY + 4, 360, 20, String.Format("<BASEFONT COLOR=#FF7700><b>{0}</b></BASEFONT>", displayName), false, false);

                // Description
                AddHtml(85, entryY + 24, 360, 36, String.Format("<BASEFONT COLOR=#222222 size=1>{0}</BASEFONT>", entry.Description), false, false);

                // Cost label
                AddHtml(450, entryY + 20, 115, 20, String.Format("<BASEFONT COLOR={0}><b>{1:N0} Feathers</b></BASEFONT>", canAfford ? "#005500" : "#B22222", entry.Cost), false, false);

                // Purchase button
                AddButton(570, entryY + 18, 0xFB7, 0xFB9, 100 + i, GumpButtonType.Reply, 0);
            }

            // Close button
            AddButton(GumpWidth / 2 - 40, GumpHeight - 34, 0x995, 0x996, 0, GumpButtonType.Reply, 0);
        }

        public static int CountFeathers(Mobile from)
        {
            return CountBackpackFeathers(from) + CountBankFeathers(from);
        }

        public static int CountBackpackFeathers(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return 0;

            return from.Backpack.GetAmount(typeof(TurkeyFeather));
        }

        public static int CountBankFeathers(Mobile from)
        {
            if (from == null || from.BankBox == null)
                return 0;

            return from.BankBox.GetAmount(typeof(TurkeyFeather));
        }

        public static bool ConsumeFeathers(Mobile from, int amount)
        {
            if (from == null || amount <= 0)
                return false;

            int packCount = CountBackpackFeathers(from);
            int bankCount = CountBankFeathers(from);

            if (packCount + bankCount < amount)
                return false;

            if (packCount >= amount)
            {
                return from.Backpack.ConsumeTotal(typeof(TurkeyFeather), amount);
            }

            int remainder = amount - packCount;

            if (packCount > 0)
            {
                from.Backpack.ConsumeTotal(typeof(TurkeyFeather), packCount);
            }

            if (from.BankBox != null && remainder > 0)
            {
                return from.BankBox.ConsumeTotal(typeof(TurkeyFeather), remainder);
            }

            return true;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

            int index = info.ButtonID - 100;
            if (index < 0 || index >= Rewards.Count)
                return;

            TurkeyRewardEntry selected = Rewards[index];

            if (from.Backpack == null)
                return;

            int feathers = CountFeathers(from);
            if (feathers < selected.Cost)
            {
                from.SendMessage(0x22, "You need {0:N0} Turkey Feathers for that, but only have {1:N0}.", selected.Cost, feathers);
                from.SendGump(new TurkeyRewardGump(from));
                return;
            }

            // Slayer Chooser branch -> open selection gump without consuming feathers yet
            if (selected.IsSlayerChooser)
            {
                from.CloseGump(typeof(TurkeySlayerSelectGump));
                from.SendGump(new TurkeySlayerSelectGump(from, selected.SlayerTarget, selected.Cost));
                return;
            }

            // Standard purchase: consume feathers
            if (!ConsumeFeathers(from, selected.Cost))
            {
                from.SendMessage(0x22, "Could not consume the required Turkey Feathers.");
                from.SendGump(new TurkeyRewardGump(from));
                return;
            }

            if (selected.IsAkalabethStatue)
            {
                StormOfAkalabethStatue statue = new StormOfAkalabethStatue();
                from.AddToBackpack(statue);
                from.PlaySound(0x5A);

                if (statue.IsUberRare)
                {
                    from.SendMessage(0x22, "★ INCREDIBLE LUCK! You received an UBER RARE Storm of Akalabeth Statuette! ★");
                    CommandHandlers.BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Harvest Festival] {0} has redeemed an UBER RARE Storm of Akalabeth Statuette!", from.Name));
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x373A, 10, 15, 5018);
                    from.PlaySound(0x209);
                }
                else
                {
                    from.SendMessage(0x35, "You have purchased a Storm of Akalabeth Statuette! Double-click it in your pack to claim your pet.");
                }
            }
            else if (selected.IsQuiver)
            {
                Item quiver;
                if (from.Race == Race.Gargoyle)
                    quiver = new GargishRiftbreakerWing();
                else
                    quiver = new RiftbreakerQuiver();

                from.AddToBackpack(quiver);
                from.PlaySound(0x2E6);
                from.FixedParticles(0x376A, 9, 32, 5030, 1161, 0, EffectLayer.Waist);
                from.SendMessage(0x35, "You have claimed the {0}!", quiver.Name);
            }
            else if (selected.IsCincture)
            {
                Item cincture;
                if (from.Race == Race.Gargoyle)
                    cincture = new GargishKaelvoksCincture();
                else
                    cincture = new KaelvoksCincture();

                from.AddToBackpack(cincture);
                from.PlaySound(0x2E6);
                from.FixedParticles(0x376A, 9, 32, 5030, 1161, 0, EffectLayer.Waist);
                from.SendMessage(0x35, "You have claimed the {0}!", cincture.Name);
            }

            from.SendGump(new TurkeyRewardGump(from));
        }
    }

    #region Slayer Selection Gump
    public class TurkeySlayerSelectGump : Gump
    {
        private readonly Mobile m_From;
        private readonly TurkeySlayerTarget m_Target;
        private readonly int m_Cost;

        private static readonly SlayerName[] m_Slayers = new SlayerName[]
        {
            SlayerName.Silver,          // Undead
            SlayerName.Exorcism,        // Demon
            SlayerName.ArachnidDoom,    // Arachnid
            SlayerName.ReptilianDeath,  // Reptile
            SlayerName.ElementalBan,    // Elemental
            SlayerName.Repond           // Repond
        };

        public TurkeySlayerSelectGump(Mobile from, TurkeySlayerTarget target, int cost) : base(120, 100)
        {
            m_From = from;
            m_Target = target;
            m_Cost = cost;

            string itemTitle;
            int artID;

            switch (target)
            {
                case TurkeySlayerTarget.DoubleAxe:
                    itemTitle = "Autumn Harvest Double Axe";
                    artID = 0xF4B;
                    break;
                case TurkeySlayerTarget.PaladinHammer:
                    itemTitle = "Autumn Harvest Paladin Hammer";
                    artID = 0x1439;
                    break;
                case TurkeySlayerTarget.Lajatang:
                    itemTitle = "Autumn Harvest Lajatang";
                    artID = 0x27A7;
                    break;
                case TurkeySlayerTarget.GargishTalwar:
                default:
                    itemTitle = "Autumn Harvest Gargish Talwar";
                    artID = 0x908;
                    break;
            }

            AddPage(0);
            AddBackground(0, 0, 420, 360, 9270);
            AddBackground(15, 15, 390, 50, 9390);

            // Autumn Brown weapon preview
            AddItem(25, 20, artID, TurkeySlayerHelper.WeaponHue);
            AddHtml(75, 20, 320, 20, String.Format("<BASEFONT COLOR=#FF7700 size=4><b>{0}</b></BASEFONT>", itemTitle), false, false);

            int feathers = TurkeyRewardGump.CountFeathers(from);
            int packFeathers = TurkeyRewardGump.CountBackpackFeathers(from);
            int bankFeathers = TurkeyRewardGump.CountBankFeathers(from);
            bool canAfford = feathers >= cost;
            string costColor = canAfford ? "#005500" : "#B22222";
            string balanceInfo = bankFeathers > 0
                ? String.Format(" (You have: <b>{0:N0}</b> [{1:N0} pack, {2:N0} bank])", feathers, packFeathers, bankFeathers)
                : String.Format(" (You have: <b>{0:N0}</b>)", feathers);

            AddHtml(75, 40, 320, 20, String.Format("<BASEFONT COLOR=#222222>Cost: <BASEFONT COLOR={0}><b>{1:N0} Feathers</b></BASEFONT>{2}</BASEFONT>", costColor, cost, balanceInfo), false, false);

            AddHtml(25, 75, 370, 35, "<BASEFONT COLOR=#C0C0C0>Choose which Slayer power you wish to imbue into your weapon:</BASEFONT>", false, false);

            int startY = 115;
            for (int i = 0; i < m_Slayers.Length; i++)
            {
                int rowY = startY + (i * 32);
                AddBackground(20, rowY, 380, 28, 9390);

                AddButton(28, rowY + 3, 0xFA5, 0xFA7, 10 + i, GumpButtonType.Reply, 0);
                AddHtml(65, rowY + 4, 120, 20, String.Format("<BASEFONT COLOR=#FFA500><b>{0} Slayer</b></BASEFONT>", TurkeySlayerHelper.GetSlayerTitle(m_Slayers[i])), false, false);
                AddHtml(185, rowY + 4, 210, 20, String.Format("<BASEFONT COLOR=#222222 size=1>({0})</BASEFONT>", TurkeySlayerHelper.GetElementalSummary(m_Slayers[i])), false, false);
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
                from.SendGump(new TurkeyRewardGump(from));
                return;
            }

            int index = info.ButtonID - 10;
            if (index < 0 || index >= m_Slayers.Length)
                return;

            SlayerName chosenSlayer = m_Slayers[index];

            if (from.Backpack == null)
                return;

            int feathers = TurkeyRewardGump.CountFeathers(from);
            if (feathers < m_Cost)
            {
                from.SendMessage(0x22, "You do not have enough Turkey Feathers ({0:N0} required).", m_Cost);
                from.SendGump(new TurkeyRewardGump(from));
                return;
            }

            if (!TurkeyRewardGump.ConsumeFeathers(from, m_Cost))
            {
                from.SendMessage(0x22, "Could not consume the required Turkey Feathers.");
                from.SendGump(new TurkeyRewardGump(from));
                return;
            }

            BaseWeapon createdWeapon = null;
            switch (m_Target)
            {
                case TurkeySlayerTarget.DoubleAxe:
                    createdWeapon = new HarvestDoubleAxe(chosenSlayer);
                    break;
                case TurkeySlayerTarget.PaladinHammer:
                    createdWeapon = new HarvestPaladinHammer(chosenSlayer);
                    break;
                case TurkeySlayerTarget.Lajatang:
                    createdWeapon = new HarvestLajatang(chosenSlayer);
                    break;
                case TurkeySlayerTarget.GargishTalwar:
                    createdWeapon = new HarvestGargishTalwar(chosenSlayer);
                    break;
            }

            if (createdWeapon == null)
            {
                from.AddToBackpack(new TurkeyFeather(m_Cost)); // refund
                from.SendMessage(0x22, "An error occurred creating your weapon. Feathers refunded.");
                return;
            }

            from.AddToBackpack(createdWeapon);
            from.PlaySound(0x5B4);
            from.FixedParticles(0x376A, 9, 32, 5030, 1, 0, EffectLayer.Waist);
            from.SendMessage(0x35, "Congratulations! You have claimed the {0}!", createdWeapon.Name);

            from.SendGump(new TurkeyRewardGump(from));
        }
    }
    #endregion
}
