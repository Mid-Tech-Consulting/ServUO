using Server.Items;
using Server.Targeting;

namespace Server.Commands
{
    public static class FixStuckTMapCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("FixStuckTMap", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("FixStuckTMap")]
        [Description("Targets a treasure map and re-rolls its chest location. Use when a player's map points under a house. The map must be re-decoded after.")]
        public static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the treasure map to relocate.");
            e.Mobile.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is TreasureMap map))
                {
                    from.SendMessage("That is not a treasure map.");
                    return;
                }

                if (map.Completed)
                {
                    from.SendMessage("That map has already been completed.");
                    return;
                }

                Point2D oldLoc = map.ChestLocation;
                map.ResetLocation();

                from.SendMessage("Map relocated. Old: ({0}, {1}) → New: ({2}, {3}). The player must redecode it.",
                    oldLoc.X, oldLoc.Y, map.ChestLocation.X, map.ChestLocation.Y);
            }
        }
    }
}
