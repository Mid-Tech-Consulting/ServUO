using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Commands
{
    // Admin diagnostic: roll a Legendary Artifact armor / clothing piece and
    // drop it in the caller's pack. Used to verify the EnforceLegendaryResistFloor
    // pass in RunicReforging actually fires on freshly-generated loot.
    public static class TestLegendaryCommand
    {
        // Pool of item types we'll randomly pick from. Mix of armor types
        // (gets BaseArmor floor branch) and clothing (gets BaseClothing branch)
        // so a few rolls exercise both code paths.
        private static readonly Type[] _Pool =
        {
            typeof(LeatherChest),
            typeof(LeatherLegs),
            typeof(LeatherCap),
            typeof(StuddedChest),
            typeof(PlateChest),
            typeof(ChainChest),
            typeof(BoneArms),
            typeof(WoodlandChest),
            typeof(Robe),
            typeof(Doublet),
            typeof(FancyShirt),
        };

        public static void Initialize()
        {
            CommandSystem.Register("testlegendary", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from.Backpack == null)
            {
                from.SendMessage(0x22, "You need a backpack.");
                return;
            }

            int attempts = 0;
            int maxAttempts = 80;
            Item legendary = null;

            while (attempts++ < maxAttempts)
            {
                Item candidate = TryConstruct(_Pool[Utility.Random(_Pool.Length)]);
                if (candidate == null)
                    continue;

                // High budget range -> reliably hits artifact tier; some rolls
                // will land Legendary. Luck 0 keeps the test clean / repeatable.
                RunicReforging.GenerateRandomItem(candidate, 0, 1100, 1200);

                ItemPower power = candidate is ICombatEquipment combat
                    ? combat.ItemPower
                    : ItemPower.None;

                if (power == ItemPower.LegendaryArtifact || power == ItemPower.ReforgedLegendary)
                {
                    legendary = candidate;
                    break;
                }

                candidate.Delete();
            }

            if (legendary != null)
            {
                from.Backpack.DropItem(legendary);
                from.SendMessage(0x40, "Legendary {0} placed in your pack (after {1} roll(s)).",
                    legendary.GetType().Name, attempts);
            }
            else
            {
                from.SendMessage(0x22, "No legendary rolled in {0} attempts. Try again or raise the budget.", maxAttempts);
            }
        }

        private static Item TryConstruct(Type t)
        {
            try
            {
                return Activator.CreateInstance(t) as Item;
            }
            catch
            {
                return null;
            }
        }
    }
}
