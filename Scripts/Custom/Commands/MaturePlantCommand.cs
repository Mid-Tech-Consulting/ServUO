using System;

using Server;
using Server.Commands;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Custom.Commands
{
    // Admin test helper: drops a fully-grown, pollinated, healthy Flax plant
    // into the caller's pack so the Reproduction gump can be checked without
    // waiting on real growth ticks. Flax (PlantType.FlaxFlowers / Plain hue)
    // is the "Peculiar" case -- crossable=false but reproduces=true -- that
    // the gump used to hide behind a bare "X".
    //
    // Usage: [matureplant            -> 4 resources + 4 seeds pre-banked
    //        [matureplant <0-8>      -> that many of each pre-banked
    //
    // Pair with [growplant to force additional growth ticks.
    public static class MaturePlantCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("matureplant", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from.Backpack == null)
            {
                from.SendMessage(0x22, "You need a backpack.");
                return;
            }

            int prebank = 4;

            if (e.Length >= 1)
                prebank = Math.Max(0, Math.Min(8, e.GetInt32(0)));

            PlantItem plant = new PlantItem();
            plant.PlantType = PlantType.FlaxFlowers;   // resources at Plain hue -> Flax
            plant.PlantHue = PlantHue.Plain;
            plant.PlantStatus = PlantStatus.Stage9;

            PlantSystem sys = plant.PlantSystem;

            if (sys != null)
            {
                sys.Hits = sys.MaxHits;          // Healthy
                sys.Pollinated = true;
                sys.SeedType = plant.PlantType;
                sys.SeedHue = plant.PlantHue;

                sys.AvailableResources = prebank;
                sys.LeftResources = 8 - prebank;
                sys.AvailableSeeds = prebank;
                sys.LeftSeeds = 8 - prebank;
            }

            plant.InvalidateProperties();
            from.Backpack.DropItem(plant);

            from.SendMessage(0x40,
                "Mature Flax plant placed in your pack -- {0}/8 resources and {0}/8 seeds banked (reproduces={1}). Double-click for the Reproduction gump.",
                prebank, plant.Reproduces);
        }
    }
}
