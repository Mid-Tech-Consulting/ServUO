using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Custom.Events
{
    // Central admin gump for all invasion themes. Each row shows one theme's
    // status (derived from InvasionLoot's active-spawner refcount), a toggle
    // button that flips every InvasionSpawner of that theme in the world,
    // and an info arrow that pages a description into the right panel.
    //
    // The per-spawner Active toggle on each placed InvasionSpawner still
    // works independently -- this is a master controller, not a replacement.
    public class InvasionAdminGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("InvasionAdmin", AccessLevel.Administrator,
                new CommandEventHandler(OnCommand));
        }

        [Usage("InvasionAdmin")]
        [Description("Opens the Invasion admin menu to toggle theme spawners.")]
        private static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump(typeof(InvasionAdminGump));
            e.Mobile.SendGump(new InvasionAdminGump());
        }

        private readonly List<InvasionTheme> m_Themes;

        public InvasionAdminGump() : base(30, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Themes = new List<InvasionTheme>(InvasionThemes.All);
            int rows = m_Themes.Count;
            int height = 75 + (rows * 25);

            // Left panel: theme table.
            AddPage(0);
            AddBackground(0, 0, 420, height, 9200);
            AddImageTiled(25, 18, 360, 10, 9267);
            AddLabel(140, 5, 54, "Invasions Admin");
            AddLabel(10, 25, 54, "Theme");
            AddLabel(165, 25, 54, "Status");
            AddLabel(235, 25, 54, "Toggle");
            AddLabel(295, 25, 54, "Spwn/Live");
            AddLabel(390, 25, 54, "Info");

            // Right panel: per-theme description, paged.
            AddBackground(420, 0, 220, 180, 9200);
            AddImageTiled(425, 5, 210, 170, 2624);
            AddAlphaRegion(425, 5, 210, 170);

            for (int i = 0; i < rows; i++)
            {
                InvasionTheme theme = m_Themes[i];
                int y = 50 + (i * 25);

                bool active = InvasionLoot.IsThemeActive(theme);

                int spawnerCount = 0;
                int liveCount = 0;
                foreach (InvasionSpawner s in SpawnersForTheme(theme))
                {
                    spawnerCount++;
                    liveCount += s.LiveCount;
                }

                int toggleBtn = active ? 2361 : 2360;

                AddLabel(10, y, 2100, theme.Name);
                AddLabel(165, y, active ? 167 : 137, active ? "Active" : "Inactive");
                AddButton(235, y + 2, toggleBtn, toggleBtn, 100 + i, GumpButtonType.Reply, 0);
                AddLabel(305, y, 2100, spawnerCount + " / " + liveCount);
                AddButton(390, y + 2, 4005, 4006, 0, GumpButtonType.Page, 1 + i);
            }

            for (int i = 0; i < rows; i++)
            {
                InvasionTheme theme = m_Themes[i];
                AddPage(1 + i);
                string body = String.Format(
                    "<center>{0}</center><br>{1}<br><br>Token: {2}<br>Reward items: {3}",
                    theme.Name,
                    theme.Description ?? "",
                    theme.TokenName,
                    theme.Rewards.Count);
                AddHtml(430, 10, 200, 160, body, false, true);
            }
        }

        private static IEnumerable<InvasionSpawner> SpawnersForTheme(InvasionTheme theme)
        {
            foreach (Item item in World.Items.Values)
            {
                InvasionSpawner s = item as InvasionSpawner;
                if (s != null && !s.Deleted && s.Theme == theme)
                    yield return s;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int button = info.ButtonID;

            if (button < 100 || button >= 100 + m_Themes.Count)
                return;

            InvasionTheme theme = m_Themes[button - 100];
            bool target = !InvasionLoot.IsThemeActive(theme);

            int total = 0;
            int changed = 0;
            foreach (InvasionSpawner s in SpawnersForTheme(theme))
            {
                total++;
                if (s.Active != target)
                {
                    s.Active = target;
                    changed++;
                }
            }

            int hue = theme.StoneHue == 0 ? 0x47E : theme.StoneHue;

            if (total == 0)
            {
                // Derive the [add command name from the theme: "Undead Invasion"
                // -> "UndeadInvasionSpawner".
                string cmdName = theme.Name.Replace(" ", "") + "Spawner";
                from.SendMessage(0x21, String.Format(
                    "No spawners exist for {0}. Place one with [add {1} first.",
                    theme.Name, cmdName));
            }
            else
            {
                from.SendMessage(hue, String.Format(
                    "{0}: {1} of {2} spawner(s) {3}.",
                    theme.Name, changed, total, target ? "activated" : "deactivated"));
            }

            from.CloseGump(typeof(InvasionAdminGump));
            from.SendGump(new InvasionAdminGump());
        }
    }
}
