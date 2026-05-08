using System;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class AnkhOfDevotionGump : Gump
    {
        private readonly PlayerMobile m_From;
        private readonly AnkhOfDevotion m_Ankh;

        // Two-column layout: Felucca shrines on the left, Tokuno on the right.
        // Matches the destination-picker style we use for Crystal / Corrupted
        // Crystal portals (dark stone background, white html labels, arrow
        // buttons next to each entry).
        public AnkhOfDevotionGump(PlayerMobile from, AnkhOfDevotion ankh)
            : base(50, 50)
        {
            m_From = from;
            m_Ankh = ankh;

            from.CloseGump(typeof(AnkhOfDevotionGump));

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(0, 0, 460, 360, 9270);

            AddHtml(0, 12, 460, 22, "<center><basefont color=#FFFFFF>Ankh of Devotion</basefont></center>", false, false);
            AddHtml(0, 36, 460, 18, "<center><basefont color=#CCCCCC>Select your destination</basefont></center>", false, false);

            // Left column: Felucca
            AddHtml(40, 64, 200, 22, "<basefont color=#FFFFAA>Felucca</basefont>", false, false);

            int y = 90;
            for (int i = 0; i < AnkhOfDevotion.FeluccaShrines.Length; i++)
            {
                var d = AnkhOfDevotion.FeluccaShrines[i];
                AddButton(40, y + 1, 0x4B9, 0x4BA, 100 + i, GumpButtonType.Reply, 0);
                AddHtml(70, y, 180, 22, "<basefont color=#FFFFFF>" + d.Label + "</basefont>", false, false);
                y += 26;
            }

            // Right column: Tokuno
            AddHtml(260, 64, 200, 22, "<basefont color=#FFFFAA>Tokuno Islands</basefont>", false, false);

            int y2 = 90;
            for (int i = 0; i < AnkhOfDevotion.TokunoShrines.Length; i++)
            {
                var d = AnkhOfDevotion.TokunoShrines[i];
                AddButton(260, y2 + 1, 0x4B9, 0x4BA, 200 + i, GumpButtonType.Reply, 0);
                AddHtml(290, y2, 160, 22, "<basefont color=#FFFFFF>" + d.Label + "</basefont>", false, false);
                y2 += 26;
            }

            // Close button (centered footer)
            AddButton(210, 320, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtml(245, 320, 100, 22, "<basefont color=#FFFFFF>Close</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted || sender.Mobile != m_From)
                return;

            if (m_Ankh == null || m_Ankh.Deleted)
                return;

            int id = info.ButtonID;
            if (id == 0)
                return;

            if (!m_From.InRange(m_Ankh.GetWorldLocation(), 3))
            {
                m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            AnkhOfDevotion.Destination dest = null;

            if (id >= 100 && id < 200)
            {
                int idx = id - 100;
                if (idx >= 0 && idx < AnkhOfDevotion.FeluccaShrines.Length)
                    dest = AnkhOfDevotion.FeluccaShrines[idx];
            }
            else if (id >= 200 && id < 300)
            {
                int idx = id - 200;
                if (idx >= 0 && idx < AnkhOfDevotion.TokunoShrines.Length)
                    dest = AnkhOfDevotion.TokunoShrines[idx];
            }

            if (dest == null)
                return;

            m_Ankh.TryTeleport(m_From, dest.Location, dest.Map);
        }
    }
}
