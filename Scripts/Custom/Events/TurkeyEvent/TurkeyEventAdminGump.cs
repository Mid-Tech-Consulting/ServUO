using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Events.TurkeyEvent
{
    public class TurkeyEventAdminGump : Gump
    {
        private const int GumpWidth = 520;
        private const int GumpHeight = 490;

        public TurkeyEventAdminGump() : base(100, 100)
        {
            AddPage(0);
            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);
            AddBackground(15, 15, GumpWidth - 30, 55, 9390);

            // Title
            AddHtml(25, 20, GumpWidth - 50, 25, "<BASEFONT COLOR=#FF7700 size=6><CENTER><b>November Giant Turkey Event Control</b></CENTER></BASEFONT>", false, false);
            string statusStr = TurkeyEvent.Enabled ? "<BASEFONT COLOR=#008000><b>ENABLED</b></BASEFONT>" : "<BASEFONT COLOR=#B22222><b>DISABLED</b></BASEFONT>";
            AddHtml(25, 45, GumpWidth - 50, 20, String.Format("<BASEFONT COLOR=#FFFFFF><CENTER>Event Status: {0} | 5 per Facet (Fel, Tokuno, Malas)</CENTER></BASEFONT>", statusStr), false, false);

            int startY = 80;

            // Toggle Button
            AddBackground(15, startY, GumpWidth - 30, 40, 9390);
            AddButton(25, startY + 8, TurkeyEvent.Enabled ? 0xFA5 : 0xFB7, TurkeyEvent.Enabled ? 0xFA7 : 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtml(65, startY + 10, 180, 20, String.Format("<BASEFONT COLOR=#222222><b>{0} Event</b></BASEFONT>", TurkeyEvent.Enabled ? "Disable" : "Enable"), false, false);

            // Respawn All Button
            AddButton(250, startY + 8, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            AddHtml(290, startY + 10, 100, 20, "<BASEFONT COLOR=#222222><b>Respawn All</b></BASEFONT>", false, false);

            // Cleanup All Button
            AddButton(395, startY + 8, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
            AddHtml(435, startY + 10, 80, 20, "<BASEFONT COLOR=#B22222><b>Kill All</b></BASEFONT>", false, false);

            // Give Reward Stone button
            int stoneRowY = startY + 46;
            AddBackground(15, stoneRowY, GumpWidth - 30, 36, 9390);
            AddButton(25, stoneRowY + 6, 0xFB7, 0xFB9, 4, GumpButtonType.Reply, 0);
            AddHtml(65, stoneRowY + 8, 420, 20, "<BASEFONT COLOR=#222222><b>Get Black Friday Reward Stone</b> (Movable stone in pack to place in Luna)</BASEFONT>", false, false);

            // Feathers Test Row
            int featherRowY = stoneRowY + 42;
            AddBackground(15, featherRowY, GumpWidth - 30, 36, 9390);
            AddButton(25, featherRowY + 6, 0xFB7, 0xFB9, 5, GumpButtonType.Reply, 0);
            AddHtml(65, featherRowY + 8, 180, 20, "<BASEFONT COLOR=#005500><b>+5,000 Feathers</b> (Test)</BASEFONT>", false, false);

            AddButton(265, featherRowY + 6, 0xFB7, 0xFB9, 6, GumpButtonType.Reply, 0);
            AddHtml(305, featherRowY + 8, 190, 20, "<BASEFONT COLOR=#005500><b>+10,000 Feathers</b> (Test)</BASEFONT>", false, false);

            // Live Turkey Counts Header
            int listY = featherRowY + 44;
            AddHtml(20, listY, 480, 20, String.Format("<BASEFONT COLOR=#FFA500><b>Active World Giant Turkeys ({0} Total):</b></BASEFONT>", TurkeyEvent.ActiveTurkeys.Count), false, false);

            int felCount = TurkeyEvent.GetAliveCount(Map.Felucca);
            int tokunoCount = TurkeyEvent.GetAliveCount(Map.Tokuno);
            int malasCount = TurkeyEvent.GetAliveCount(Map.Malas);

            int statRowY = listY + 22;
            AddBackground(15, statRowY, GumpWidth - 30, 30, 9390);
            AddHtml(25, statRowY + 6, 470, 20, String.Format("<BASEFONT COLOR=#222222>Felucca: <b>{0}/5</b>   |   Tokuno: <b>{1}/5</b>   |   Malas: <b>{2}/5</b></BASEFONT>", felCount, tokunoCount, malasCount), false, false);

            // Active Turkeys list (up to 5 shown with teleport buttons)
            int entriesY = statRowY + 36;
            var list = TurkeyEvent.ActiveTurkeys;
            int count = Math.Min(5, list.Count);

            for (int i = 0; i < count; i++)
            {
                EventGiantTurkey t = list[i];
                if (t == null || t.Deleted)
                    continue;

                int rowY = entriesY + (i * 26);
                AddBackground(15, rowY, GumpWidth - 30, 24, 9390);

                AddButton(25, rowY + 2, 0xFA5, 0xFA7, 100 + i, GumpButtonType.Reply, 0);
                string mapName = t.Map != null ? t.Map.Name : "Unknown";
                AddHtml(65, rowY + 2, 420, 20, String.Format("<BASEFONT COLOR=#222222><b>{0}</b> at ({1}, {2}, {3}) [{4}] - {5:N0}/{6:N0} HP</BASEFONT>", t.Name, t.X, t.Y, t.Z, mapName, t.Hits, t.HitsMax), false, false);
            }

            // Close button
            AddButton(GumpWidth / 2 - 40, GumpHeight - 32, 0x995, 0x996, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            if (from.AccessLevel < AccessLevel.Counselor)
                return;

            switch (info.ButtonID)
            {
                case 1: // Toggle Enabled
                    TurkeyEvent.Enabled = !TurkeyEvent.Enabled;
                    from.SendMessage(0x35, "November Giant Turkey Event is now {0}.", TurkeyEvent.Enabled ? "ENABLED" : "DISABLED");
                    if (TurkeyEvent.Enabled)
                        TurkeyEvent.CheckSpawns();
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                case 2: // Respawn All
                    TurkeyEvent.RespawnAll();
                    from.SendMessage(0x35, "Respawned all Giant Turkeys across Felucca, Tokuno, and Malas forests.");
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                case 3: // Cleanup All
                    TurkeyEvent.CleanupTurkeys();
                    from.SendMessage(0x22, "Cleaned up all active event Giant Turkeys.");
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                case 4: // Give Reward Stone
                    from.AddToBackpack(new TurkeyRewardStone());
                    from.SendMessage(0x35, "A Black Friday Turkey Reward Stone has been placed in your backpack. You can move it or double-click it to target where to place it in Luna!");
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                case 5: // Get 5,000 Feathers
                    from.AddToBackpack(new TurkeyFeather(5000));
                    from.SendMessage(0x35, "Added 5,000 Turkey Feathers to your backpack for testing!");
                    from.PlaySound(0x2E6);
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                case 6: // Get 10,000 Feathers
                    from.AddToBackpack(new TurkeyFeather(10000));
                    from.SendMessage(0x35, "Added 10,000 Turkey Feathers to your backpack for testing!");
                    from.PlaySound(0x2E6);
                    from.SendGump(new TurkeyEventAdminGump());
                    break;

                default:
                    int index = info.ButtonID - 100;
                    if (index >= 0 && index < TurkeyEvent.ActiveTurkeys.Count)
                    {
                        EventGiantTurkey target = TurkeyEvent.ActiveTurkeys[index];
                        if (target != null && !target.Deleted && target.Map != null)
                        {
                            from.MoveToWorld(target.Location, target.Map);
                            from.SendMessage(0x35, "Teleported to Giant Turkey in {0}!", target.Map.Name);
                        }
                    }
                    break;
            }
        }
    }
}
