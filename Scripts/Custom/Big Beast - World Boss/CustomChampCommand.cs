using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Engines.CannedEvil;

namespace Server.Custom.Events
{
    public class CustomChampCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CustomChampAdmin", AccessLevel.GameMaster,
                new CommandEventHandler(OnCommand));
            CommandSystem.Register("ChampAdmin", AccessLevel.GameMaster,
                new CommandEventHandler(OnCommand));
        }

        [Usage("CustomChampAdmin")]
        [Description("Opens the Custom Champion Spawn admin menu to place themed spawners.")]
        private static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump(typeof(CustomChampAdminGump));
            e.Mobile.SendGump(new CustomChampAdminGump(e.Mobile));
        }
    }

    public class CustomChampAdminGump : Gump
    {
        private readonly List<CustomChampTheme> m_Themes;
        private readonly Mobile m_From;

        public CustomChampAdminGump(Mobile from) : base(50, 50)
        {
            m_From = from;
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_Themes = new List<CustomChampTheme>(CustomChampThemes.All);
            int rows = m_Themes.Count;
            int height = 75 + (rows * 30);

            AddPage(0);
            AddBackground(0, 0, 450, height, 9200);
            AddImageTiled(25, 18, 400, 10, 9267);
            AddLabel(150, 5, 54, "Custom Champ Admin");
            AddLabel(15, 25, 54, "Spawn Theme");
            AddLabel(180, 25, 54, "Spawn ID");
            AddLabel(260, 25, 54, "Artifacts");
            AddLabel(370, 25, 54, "Spawn Altar");

            for (int i = 0; i < rows; i++)
            {
                CustomChampTheme theme = m_Themes[i];
                int y = 50 + (i * 30);

                AddLabel(15, y, 2100, theme.Name);
                AddLabel(180, y, 2100, ((int)theme.SpawnType).ToString());
                AddLabel(260, y, 2100, String.Format("{0} rewards", theme.UniqueList.Length + theme.SharedList.Length + theme.DecorativeList.Length));
                
                // Add button: button ID is 100 + index
                AddButton(370, y + 2, 4005, 4006, 100 + i, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int button = info.ButtonID;

            if (button < 100 || button >= 100 + m_Themes.Count)
                return;

            CustomChampTheme theme = m_Themes[button - 100];

            // Spawn the altar at the player's feet
            ChampionSpawn spawn = new ChampionSpawn();
            spawn.Type = theme.SpawnType;
            
            // ChampionSpawn platform is at Z-20, altar is at Z-15.
            // Placing the spawner at Z+20 ensures the platform and altar sit exactly on the ground level.
            Point3D spawnLoc = new Point3D(from.X, from.Y, from.Z + 20);
            spawn.MoveToWorld(spawnLoc, from.Map);
            ChampionSystem.AllSpawns.Add(spawn);

            CommandLogging.WriteLine(from, "{0} {1} spawned a {2} Champion Spawn Altar at {3} on map {4}", from.AccessLevel, from.Name, theme.Name, from.Location, from.Map);
            from.SendMessage(0x42, String.Format("Successfully placed a {0} Champion Altar at your feet!", theme.Name));

            // Re-open the gump
            from.SendGump(new CustomChampAdminGump(from));
        }
    }
}
