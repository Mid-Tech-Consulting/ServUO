using System;

using Server;
using Server.Items;

namespace Server.Custom.Loot
{
    // Shared "give me a Legendary Artifact armor / clothing piece" helper.
    // Used by [testlegendary and by the Ancient SOS chest fill -- anywhere we
    // want a guaranteed legendary instead of an RNG roll on the loot table.
    //
    // Each attempt spawns a fresh armor or clothing piece and pushes
    // RunicReforging.GenerateRandomItem at a high budget (1100-1200). Most
    // rolls land Lesser/Greater/Major artifact; some come out Legendary. The
    // caller picks how many attempts to spend; 80 is roughly the point where
    // missing a Legendary becomes vanishingly unlikely.
    public static class LegendaryRoller
    {
        // Armor pieces + jewelry only -- no clothing. Each material is
        // represented across the major slots so legendaries feel varied.
        // Gargoyle equivalents are included alongside human-side pieces so
        // garg players see legendaries that fit their race.
        private static readonly Type[] _Pool =
        {
            // --- Human armor ---
            // Leather
            typeof(LeatherChest), typeof(LeatherLegs), typeof(LeatherArms),
            typeof(LeatherGloves), typeof(LeatherGorget), typeof(LeatherCap),
            // Studded
            typeof(StuddedChest), typeof(StuddedLegs), typeof(StuddedArms),
            typeof(StuddedGloves), typeof(StuddedGorget),
            // Bone
            typeof(BoneChest), typeof(BoneLegs), typeof(BoneArms),
            typeof(BoneGloves), typeof(BoneHelm),
            // Ringmail
            typeof(RingmailChest), typeof(RingmailLegs), typeof(RingmailArms),
            typeof(RingmailGloves),
            // Chain
            typeof(ChainChest), typeof(ChainLegs), typeof(ChainCoif),
            // Plate
            typeof(PlateChest), typeof(PlateLegs), typeof(PlateArms),
            typeof(PlateGloves), typeof(PlateGorget), typeof(PlateHelm),
            // Woodland
            typeof(WoodlandChest), typeof(WoodlandLegs), typeof(WoodlandArms),
            typeof(WoodlandGloves), typeof(WoodlandGorget),
            // Dragon
            typeof(DragonChest), typeof(DragonLegs), typeof(DragonArms),
            typeof(DragonGloves), typeof(DragonHelm),

            // --- Gargoyle armor ---
            // Leather
            typeof(GargishLeatherChest), typeof(GargishLeatherLegs),
            typeof(GargishLeatherArms), typeof(GargishLeatherKilt),
            typeof(GargishLeatherWingArmor),
            // Stone
            typeof(GargishStoneChest), typeof(GargishStoneLegs),
            typeof(GargishStoneArms), typeof(GargishStoneKilt),
            // Plate
            typeof(GargishPlateChest), typeof(GargishPlateLegs),
            typeof(GargishPlateArms), typeof(GargishPlateKilt),
            // Cloth-armor (still BaseArmor; gargoyle-only mage gear)
            typeof(GargishClothChestArmor), typeof(GargishClothLegsArmor),
            typeof(GargishClothArmsArmor), typeof(GargishClothKiltArmor),

            // --- Jewelry ---
            typeof(GoldRing), typeof(SilverRing),
            typeof(GoldBracelet), typeof(SilverBracelet),
            typeof(GoldEarrings), typeof(SilverEarrings),
            // Gargoyle jewelry
            typeof(GargishRing), typeof(GargishBracelet), typeof(GargishEarrings),

            // --- Weapons (human-side) ---
            // Swords
            typeof(Longsword), typeof(Broadsword), typeof(Katana), typeof(Cutlass),
            typeof(Scimitar), typeof(VikingSword), typeof(BoneHarvester),
            // Fencing
            typeof(Kryss), typeof(ShortSpear), typeof(Spear), typeof(WarFork), typeof(Pike),
            // Maces
            typeof(Mace), typeof(WarMace), typeof(WarHammer), typeof(Maul),
            typeof(QuarterStaff), typeof(BlackStaff), typeof(GnarledStaff),
            // Axes
            typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe),
            typeof(LargeBattleAxe), typeof(TwoHandedAxe), typeof(Bardiche), typeof(Halberd),
            // Bows
            typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow),
            typeof(MagicalShortbow), typeof(RepeatingCrossbow), typeof(Yumi),

            // --- Gargoyle weapons ---
            typeof(DiscMace), typeof(GargishKryss), typeof(GargishWarFork),
            typeof(GargishWarHammer), typeof(GargishMaul),
            typeof(GargishAxe), typeof(GargishBattleAxe),
            typeof(BloodBlade), typeof(GlassSword),
            typeof(SoulGlaive), typeof(Cyclone), typeof(Boomerang),
        };

        /// <summary>
        /// Rolls until a Legendary Artifact is produced or maxAttempts is
        /// exhausted. Returns the legendary item on success, null on failure.
        /// On failure no item is returned -- the caller decides whether to
        /// fall back to a non-legendary or skip.
        /// </summary>
        public static Item TryRollLegendary(int maxAttempts = 80, int minBudget = 1100, int maxBudget = 1200)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                Item candidate = TryConstruct(_Pool[Utility.Random(_Pool.Length)]);

                if (candidate == null)
                    continue;

                RunicReforging.GenerateRandomItem(candidate, 0, minBudget, maxBudget);

                ItemPower power = candidate is ICombatEquipment combat
                    ? combat.ItemPower
                    : ItemPower.None;

                if (power == ItemPower.LegendaryArtifact || power == ItemPower.ReforgedLegendary)
                    return candidate;

                candidate.Delete();
            }

            return null;
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
