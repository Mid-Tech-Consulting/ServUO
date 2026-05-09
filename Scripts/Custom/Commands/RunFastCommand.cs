using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Network;

namespace Server.Custom.Commands
{
    // [runfast toggles mount-speed-on-foot for the calling GM. The SpeedControl
    // packet is what tar potions / paralysis / mount riding all use; sending
    // MountSpeed gives the player horse pace while walking. State is tracked
    // here so a second [runfast cleanly disables it instead of stacking.
    public static class RunFastCommand
    {
        private static readonly HashSet<Mobile> _Active = new HashSet<Mobile>();

        public static void Initialize()
        {
            CommandSystem.Register("runfast", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m.NetState == null)
                return;

            if (_Active.Remove(m))
            {
                m.SendSpeedControl(SpeedControlType.Disable);
                m.SendMessage(0x40, "Run-fast disabled.");
            }
            else
            {
                _Active.Add(m);
                m.SendSpeedControl(SpeedControlType.MountSpeed);
                m.SendMessage(0x40, "Run-fast enabled (mount speed on foot).");
            }
        }
    }
}
