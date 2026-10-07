using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class HalloweenPumpkinSplitGump : Gump
    {
        private readonly Mobile m_From;
        private readonly HalloweenEventPumpkin m_Pumpkin;

        private const int GumpWidth = 470;
        private const int GumpHeight = 280;

        public HalloweenPumpkinSplitGump(Mobile from, HalloweenEventPumpkin pumpkin) : base(100, 100)
        {
            m_From = from;
            m_Pumpkin = pumpkin;

            if (from == null || pumpkin == null || pumpkin.Deleted)
                return;

            AddPage(0);
            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);
            AddBackground(15, 15, GumpWidth - 30, 56, 9390);

            // Icon & Header
            AddItem(25, 20, 0x4694);
            AddHtml(75, 20, 360, 25, "<BASEFONT COLOR=#FF7700 size=5><b>Split Halloween Pumpkins</b></BASEFONT>", false, false);

            double totalWeight = pumpkin.Amount * pumpkin.Weight;
            AddHtml(75, 45, 360, 20, String.Format("<BASEFONT COLOR=#C0C0C0>Stack: <BASEFONT COLOR=#FFFFFF><b>{0:N0}</b></BASEFONT> pumpkins ({1:N1} stones)</BASEFONT>", pumpkin.Amount, totalWeight), false, false);

            // Presets Header
            AddHtml(25, 80, 420, 20, "<BASEFONT COLOR=#FFA500><b>Quick Presets (Split to Backpack):</b></BASEFONT>", false, false);

            // Row 1 Presets
            // 150 (Dyes/Bag)
            AddButton(25, 105, 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
            AddHtml(50, 106, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>150</b></BASEFONT>", false, false);

            // 450 (Mask/Sash)
            AddButton(105, 105, 0xFA5, 0xFA7, 11, GumpButtonType.Reply, 0);
            AddHtml(130, 106, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>450</b></BASEFONT>", false, false);

            // 600 (Lantern)
            AddButton(185, 105, 0xFA5, 0xFA7, 12, GumpButtonType.Reply, 0);
            AddHtml(210, 106, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>600</b></BASEFONT>", false, false);

            // 750 (Mantle)
            AddButton(265, 105, 0xFA5, 0xFA7, 13, GumpButtonType.Reply, 0);
            AddHtml(290, 106, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>750</b></BASEFONT>", false, false);

            // 900 (Slayer)
            AddButton(345, 105, 0xFA5, 0xFA7, 14, GumpButtonType.Reply, 0);
            AddHtml(370, 106, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>900</b></BASEFONT>", false, false);

            // Row 2 Presets
            // 1,000 (Deeds)
            AddButton(25, 135, 0xFA5, 0xFA7, 15, GumpButtonType.Reply, 0);
            AddHtml(50, 136, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>1,000</b></BASEFONT>", false, false);

            // 5,000
            AddButton(105, 135, 0xFA5, 0xFA7, 16, GumpButtonType.Reply, 0);
            AddHtml(130, 136, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>5,000</b></BASEFONT>", false, false);

            // 10,000 (Dragon)
            AddButton(185, 135, 0xFA5, 0xFA7, 17, GumpButtonType.Reply, 0);
            AddHtml(210, 136, 60, 20, "<BASEFONT COLOR=#FFFFFF><b>10,000</b></BASEFONT>", false, false);

            // Half Stack
            AddButton(265, 135, 0xFA5, 0xFA7, 18, GumpButtonType.Reply, 0);
            AddHtml(290, 136, 50, 20, "<BASEFONT COLOR=#FFFFFF><b>Half</b></BASEFONT>", false, false);

            // Max Carry
            int maxCarry = CalculateMaxCarryable(from, pumpkin);
            AddButton(345, 135, 0xFA5, 0xFA7, 19, GumpButtonType.Reply, 0);
            AddHtml(370, 136, 85, 20, String.Format("<BASEFONT COLOR=#FFFFFF><b>Max ({0:N0})</b></BASEFONT>", maxCarry), false, false);

            // Custom Amount Header & Input
            AddHtml(25, 170, 420, 20, "<BASEFONT COLOR=#FFA500><b>Or Enter Custom Amount:</b></BASEFONT>", false, false);

            AddBackground(25, 195, 110, 26, 0x2486);
            AddTextEntry(30, 198, 100, 20, 0x480, 1, "100");

            // Custom Action Buttons
            AddButton(155, 195, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtml(190, 197, 135, 20, "<BASEFONT COLOR=#FFFFFF><b>To Backpack</b></BASEFONT>", false, false);

            AddButton(315, 195, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0);
            AddHtml(350, 197, 110, 20, "<BASEFONT COLOR=#C0C0C0>In Container</BASEFONT>", false, false);

            // Cancel Button
            AddButton(195, 238, 0x995, 0x996, 0, GumpButtonType.Reply, 0);
        }

        private static int ParseAmount(RelayInfo info)
        {
            if (info == null)
                return 0;

            TextRelay entry = info.GetTextEntry(1);
            if (entry == null || String.IsNullOrWhiteSpace(entry.Text))
                return 0;

            string raw = entry.Text.Trim().Replace(",", "");
            int val;
            if (int.TryParse(raw, out val))
            {
                return val;
            }

            return 0;
        }

        public static int CalculateMaxCarryable(Mobile from, HalloweenEventPumpkin pumpkin)
        {
            if (from == null || pumpkin == null || pumpkin.Deleted)
                return 0;

            int freePlayerWeight = Math.Max(0, from.MaxWeight - from.TotalWeight);
            int freePackWeight = 400;

            if (from.Backpack != null)
            {
                freePackWeight = Math.Max(0, from.Backpack.MaxWeight - from.Backpack.TotalWeight);
            }

            int freeWeight = Math.Min(freePlayerWeight, freePackWeight);
            double itemWeight = pumpkin.Weight > 0.0 ? pumpkin.Weight : 0.01;

            int maxByWeight = (int)(freeWeight / itemWeight);
            int maxPumpkins = Math.Min(pumpkin.Amount - 1, maxByWeight);

            return Math.Max(0, maxPumpkins);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || m_Pumpkin == null || m_Pumpkin.Deleted)
                return;

            int splitAmount = 0;
            bool toBackpack = true;

            switch (info.ButtonID)
            {
                case 1: // Custom -> Backpack
                    splitAmount = ParseAmount(info);
                    toBackpack = true;
                    break;

                case 2: // Custom -> Container
                    splitAmount = ParseAmount(info);
                    toBackpack = false;
                    break;

                case 10: splitAmount = 150; toBackpack = true; break;
                case 11: splitAmount = 450; toBackpack = true; break;
                case 12: splitAmount = 600; toBackpack = true; break;
                case 13: splitAmount = 750; toBackpack = true; break;
                case 14: splitAmount = 900; toBackpack = true; break;
                case 15: splitAmount = 1000; toBackpack = true; break;
                case 16: splitAmount = 5000; toBackpack = true; break;
                case 17: splitAmount = 10000; toBackpack = true; break;
                case 18:
                    splitAmount = m_Pumpkin.Amount / 2;
                    toBackpack = true;
                    break;
                case 19:
                    splitAmount = CalculateMaxCarryable(from, m_Pumpkin);
                    toBackpack = true;
                    if (splitAmount <= 0)
                    {
                        from.SendMessage(0x22, "You are currently carrying too much weight to split any pumpkins into your backpack.");
                        from.SendGump(new HalloweenPumpkinSplitGump(from, m_Pumpkin));
                        return;
                    }
                    break;
                default:
                    return;
            }

            if (splitAmount <= 0)
            {
                from.SendMessage(0x22, "Please enter a valid positive number.");
                from.SendGump(new HalloweenPumpkinSplitGump(from, m_Pumpkin));
                return;
            }

            if (!m_Pumpkin.Split(from, splitAmount, toBackpack))
            {
                if (!m_Pumpkin.Deleted && m_Pumpkin.Amount > 1)
                {
                    from.SendGump(new HalloweenPumpkinSplitGump(from, m_Pumpkin));
                }
            }
        }
    }
}
