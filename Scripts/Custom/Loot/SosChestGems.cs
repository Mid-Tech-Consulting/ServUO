using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;

namespace Server.Custom.Loot
{
    // Helper for the SOS / shipwreck chest gem stash.
    //
    // TreasureMapChest.Fill drops gems via cont.DropItem(Loot.RandomGem()) one
    // at a time, so the chest ends up with a pile of loose singleton gems
    // alongside whatever pre-stacked stash we add. This helper walks any
    // already-loose gems matching Server.Loot.GemTypes, sums them per type, deletes
    // the singletons, and drops a single consolidated stack per type that
    // bundles the per-stack baseline plus the salvaged amount.
    public static class SosChestGems
    {
        public static void AddGuaranteedGemStacks(Container chest, int perStack = 25)
        {
            if (chest == null)
                return;

            HashSet<Type> gemTypes = new HashSet<Type>(Server.Loot.GemTypes);

            Dictionary<Type, int> totals = new Dictionary<Type, int>();
            foreach (Type t in Server.Loot.GemTypes)
                totals[t] = perStack;

            // Sweep existing loose gems out of the chest (including any
            // already-stacked ones) and roll their Amounts into the totals.
            List<Item> loose = chest.Items.Where(i => gemTypes.Contains(i.GetType())).ToList();
            foreach (Item gem in loose)
            {
                totals[gem.GetType()] += gem.Stackable ? gem.Amount : 1;
                gem.Delete();
            }

            foreach (KeyValuePair<Type, int> kv in totals)
            {
                Item stack = Activator.CreateInstance(kv.Key) as Item;
                if (stack == null)
                    continue;

                if (stack.Stackable)
                    stack.Amount = kv.Value;

                chest.DropItem(stack);
            }
        }
    }
}
