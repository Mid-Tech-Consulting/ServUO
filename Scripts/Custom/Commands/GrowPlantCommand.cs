using System;

using Server;
using Server.Commands;
using Server.Engines.Plants;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Commands
{
    // Admin test helper: force one or more growth ticks on a targeted plant
    // instead of waiting on the 23-hour growth clock. Each forced tick keeps
    // the plant healthy, watered, and malady-free so it actually produces
    // (Grow() skips production on an unhealthy plant). Reports the resulting
    // stage and the resource / seed pools so the seed-display fix can be
    // verified end to end.
    //
    // Usage: [growplant            -> 1 forced tick
    //        [growplant <1-30>     -> that many forced ticks
    public static class GrowPlantCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("growplant", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            int ticks = 1;

            if (e.Length >= 1)
                ticks = Math.Max(1, Math.Min(30, e.GetInt32(0)));

            e.Mobile.SendMessage(0x40, "Target the plant to force-grow ({0} tick{1}).", ticks, ticks == 1 ? "" : "s");
            e.Mobile.Target = new GrowTarget(ticks);
        }

        private class GrowTarget : Target
        {
            private readonly int m_Ticks;

            public GrowTarget(int ticks)
                : base(10, false, TargetFlags.None)
            {
                m_Ticks = ticks;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is PlantItem plant))
                {
                    from.SendMessage(0x22, "That's not a plant.");
                    return;
                }

                PlantSystem sys = plant.PlantSystem;

                if (sys == null)
                {
                    from.SendMessage(0x22, "That plant has no growth system (it may already be a decorative plant).");
                    return;
                }

                int done = 0;

                for (int i = 0; i < m_Ticks; i++)
                {
                    sys = plant.PlantSystem; // PlantStatus setter can swap the system out
                    if (sys == null)
                        break;

                    // Pristine, watered, no maladies -- Grow() bails on production
                    // for an unhealthy plant, so we keep it perfect for the test.
                    sys.Hits = sys.MaxHits;
                    sys.Water = 1;
                    sys.Infestation = 0;
                    sys.Fungus = 0;
                    sys.Disease = 0;
                    sys.Poison = 0;

                    sys.NextGrowth = DateTime.UtcNow.AddSeconds(-1);
                    sys.DoGrowthCheck();
                    done++;

                    if (plant.Deleted || plant.PlantStatus >= PlantStatus.DecorativePlant)
                        break;
                }

                plant.InvalidateProperties();

                sys = plant.PlantSystem;

                if (sys != null)
                {
                    from.SendMessage(0x40,
                        "Forced {0} tick(s). Stage {1} | resources {2}/{3} | seeds {4}/{5} | pollinated={6} reproduces={7}.",
                        done, plant.PlantStatus,
                        sys.AvailableResources, sys.AvailableResources + sys.LeftResources,
                        sys.AvailableSeeds, sys.AvailableSeeds + sys.LeftSeeds,
                        sys.Pollinated, plant.Reproduces);
                }
                else
                {
                    from.SendMessage(0x40, "Forced {0} tick(s). Plant is now stage {1}.", done, plant.PlantStatus);
                }
            }
        }
    }
}
