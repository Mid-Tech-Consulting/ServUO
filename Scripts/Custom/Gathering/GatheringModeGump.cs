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

            AddBackground(0, 0, 280, 350, 9270);

            AddHtml(0, 14, 280, 22, "<center><basefont color=#FFFFFF>Gathering Mode</basefont></center>", false, false);

            GatheringMode current = GatheringModeSystem.GetMode(owner);
            string status = current == GatheringMode.None
                ? "<basefont color=#888888>Currently disabled</basefont>"
                : "<basefont color=#80FF80>Currently active: " + current + "</basefont>";

            AddHtml(0, 38, 280, 18, "<center>" + status + "</center>", false, false);

            AddHtml(0, 64, 280, 18, "<center><basefont color=#CCCCCC>Stand near and gather automatically.</basefont></center>", false, false);
            AddHtml(0, 82, 280, 18, "<center><basefont color=#999999>(Tools must be in your backpack.)</basefont></center>", false, false);

            // Mining
            AddButton(40, 110, 0x4B9, 0x4BA, 1, GumpButtonType.Reply, 0);
            AddHtml(70, 109, 200, 22, "<basefont color=#FFFFFF>Mining</basefont>", false, false);

            // Lumberjacking
            AddButton(40, 138, 0x4B9, 0x4BA, 2, GumpButtonType.Reply, 0);
            AddHtml(70, 137, 200, 22, "<basefont color=#FFFFFF>Lumberjacking</basefont>", false, false);

            // Off
            AddButton(40, 166, 0x4B9, 0x4BA, 3, GumpButtonType.Reply, 0);
            AddHtml(70, 165, 200, 22, "<basefont color=#FF8080>Off</basefont>", false, false);

            // Auto-cut logs to boards toggle (lumberjacking-only feature)
            bool autoCut = GatheringModeSystem.IsAutoCutLogs(owner);
            AddCheck(40, 200, 0xD2, 0xD3, autoCut, 100);
            AddHtml(70, 199, 200, 22, "<basefont color=#FFFFFF>Auto-cut logs to boards</basefont>", false, false);

            AddHtml(40, 222, 220, 18, "<basefont color=#999999>(applies while lumberjacking)</basefont>", false, false);

            // Auto-create tools toggle (uses player's Tinkering skill + iron ingots)
            bool autoCreate = GatheringModeSystem.IsAutoCreateTools(owner);
            AddCheck(40, 246, 0xD2, 0xD3, autoCreate, 101);
            AddHtml(70, 245, 200, 22, "<basefont color=#FFFFFF>Auto-create tools</basefont>", false, false);

            AddHtml(40, 268, 220, 18, "<basefont color=#999999>(needs Tinkering + iron ingots)</basefont>", false, false);

            // Auto-smelt ores toggle (mining-only feature; needs forge or fire beetle)
            bool autoSmelt = GatheringModeSystem.IsAutoSmelt(owner);
            AddCheck(40, 290, 0xD2, 0xD3, autoSmelt, 102);
            AddHtml(70, 289, 200, 22, "<basefont color=#FFFFFF>Auto-smelt ores</basefont>", false, false);

            AddHtml(40, 312, 220, 18, "<basefont color=#999999>(needs forge or fire beetle nearby)</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Owner == null || m_Owner.Deleted || sender.Mobile != m_Owner)
                return;

            // Persist the toggle states on every response (closing the gump,
            // clicking a mode button, etc.) so the checkboxes always stick.
            bool autoCut = false;
            bool autoCreate = false;
            bool autoSmelt = false;
            for (int i = 0; i < info.Switches.Length; i++)
            {
                if (info.Switches[i] == 100)
                    autoCut = true;
                else if (info.Switches[i] == 101)
                    autoCreate = true;
                else if (info.Switches[i] == 102)
                    autoSmelt = true;
            }
            GatheringModeSystem.SetAutoCutLogs(m_Owner, autoCut);
            GatheringModeSystem.SetAutoCreateTools(m_Owner, autoCreate);
            GatheringModeSystem.SetAutoSmelt(m_Owner, autoSmelt);

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
            }
        }
    }
}
