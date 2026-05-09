using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Gathering
{
    public class GatheringModeGump : Gumps.Gump
    {
        private readonly PlayerMobile m_Owner;

        public GatheringModeGump(PlayerMobile owner) : base(150, 150)
        {
            m_Owner = owner;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(0, 0, 290, 420, 9270);

            AddHtml(0, 14, 290, 22, "<center><basefont color=#FFFFFF>Gathering Mode</basefont></center>", false, false);

            GatheringMode current = GatheringModeSystem.GetMode(owner);
            string status = current == GatheringMode.None
                ? "<basefont color=#888888>Currently disabled</basefont>"
                : "<basefont color=#80FF80>Currently active: " + current + "</basefont>";

            AddHtml(0, 38, 290, 18, "<center>" + status + "</center>", false, false);

            AddHtml(0, 62, 290, 18, "<center><basefont color=#CCCCCC>Stand near and gather automatically.</basefont></center>", false, false);
            AddHtml(0, 80, 290, 18, "<center><basefont color=#999999>(Tools must be in your backpack.)</basefont></center>", false, false);

            // ----- Mode buttons -----
            // Mining
            AddButton(40, 108, 0x4B9, 0x4BA, 1, GumpButtonType.Reply, 0);
            AddHtml(70, 107, 200, 22, "<basefont color=#FFFFFF>Mining</basefont>", false, false);

            // Lumberjacking
            AddButton(40, 134, 0x4B9, 0x4BA, 2, GumpButtonType.Reply, 0);
            AddHtml(70, 133, 200, 22, "<basefont color=#FFFFFF>Lumberjacking</basefont>", false, false);

            // Fishing
            AddButton(40, 160, 0x4B9, 0x4BA, 4, GumpButtonType.Reply, 0);
            AddHtml(70, 159, 200, 22, "<basefont color=#FFFFFF>Fishing</basefont>", false, false);

            // Off
            AddButton(40, 186, 0x4B9, 0x4BA, 3, GumpButtonType.Reply, 0);
            AddHtml(70, 185, 200, 22, "<basefont color=#FF8080>Off</basefont>", false, false);

            // ----- Toggles -----
            // Auto-cut logs to boards (lumberjacking)
            bool autoCutLogs = GatheringModeSystem.IsAutoCutLogs(owner);
            AddCheck(40, 220, 0xD2, 0xD3, autoCutLogs, 100);
            AddHtml(70, 219, 220, 22, "<basefont color=#FFFFFF>Auto-cut logs to boards</basefont>", false, false);
            AddHtml(40, 240, 240, 18, "<basefont color=#999999>(applies while lumberjacking)</basefont>", false, false);

            // Auto-create tools (mining + lumberjacking)
            bool autoCreate = GatheringModeSystem.IsAutoCreateTools(owner);
            AddCheck(40, 262, 0xD2, 0xD3, autoCreate, 101);
            AddHtml(70, 261, 220, 22, "<basefont color=#FFFFFF>Auto-create tools</basefont>", false, false);
            AddHtml(40, 282, 240, 18, "<basefont color=#999999>(needs Tinkering + iron ingots)</basefont>", false, false);

            // Auto-smelt ores (mining)
            bool autoSmelt = GatheringModeSystem.IsAutoSmelt(owner);
            AddCheck(40, 304, 0xD2, 0xD3, autoSmelt, 102);
            AddHtml(70, 303, 220, 22, "<basefont color=#FFFFFF>Auto-smelt ores</basefont>", false, false);
            AddHtml(40, 324, 240, 18, "<basefont color=#999999>(needs forge or fire beetle nearby)</basefont>", false, false);

            // Auto-cut fish to steaks (fishing)
            bool autoCutFish = GatheringModeSystem.IsAutoCutFish(owner);
            AddCheck(40, 346, 0xD2, 0xD3, autoCutFish, 103);
            AddHtml(70, 345, 220, 22, "<basefont color=#FFFFFF>Auto-cut fish to steaks</basefont>", false, false);
            AddHtml(40, 366, 240, 18, "<basefont color=#999999>(toggle off to keep quest fish)</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Owner == null || m_Owner.Deleted || sender.Mobile != m_Owner)
                return;

            // Persist the toggle states on every response so the checkboxes
            // always stick regardless of which button (mode or close) is used.
            bool autoCutLogs = false;
            bool autoCreate = false;
            bool autoSmelt = false;
            bool autoCutFish = false;
            for (int i = 0; i < info.Switches.Length; i++)
            {
                if (info.Switches[i] == 100) autoCutLogs = true;
                else if (info.Switches[i] == 101) autoCreate = true;
                else if (info.Switches[i] == 102) autoSmelt = true;
                else if (info.Switches[i] == 103) autoCutFish = true;
            }
            GatheringModeSystem.SetAutoCutLogs(m_Owner, autoCutLogs);
            GatheringModeSystem.SetAutoCreateTools(m_Owner, autoCreate);
            GatheringModeSystem.SetAutoSmelt(m_Owner, autoSmelt);
            GatheringModeSystem.SetAutoCutFish(m_Owner, autoCutFish);

            switch (info.ButtonID)
            {
                case 1:
                    GatheringModeSystem.SetMode(m_Owner, GatheringMode.Mining);
                    break;
                case 2:
                    GatheringModeSystem.SetMode(m_Owner, GatheringMode.Lumberjacking);
                    break;
                case 3:
                    GatheringModeSystem.SetMode(m_Owner, GatheringMode.None);
                    break;
                case 4:
                    GatheringModeSystem.SetMode(m_Owner, GatheringMode.Fishing);
                    break;
            }
        }
    }
}
