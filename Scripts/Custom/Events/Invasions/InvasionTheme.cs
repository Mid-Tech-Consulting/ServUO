using System;
using System.Collections.Generic;

using Server;

namespace Server.Custom.Events
{
    // POCO that describes a single Invasion theme (Undead / Demon / Reptile
    // / Arachnid / Fey / ...). Everything that varies between events lives
    // here: token type, display name, mob bounty table, spawner population
    // mix, reward catalog, and the cosmetic IDs for the stone/spawner.
    //
    // To add a new theme: instantiate one of these in InvasionThemes and
    // wire it into the registry. No engine code needs to change.
    public class InvasionTheme
    {
        public string Name;                                  // "Undead Invasion"
        public string Description;                           // shown on the reward stone

        public Type TokenType;                               // typeof(BoneToken)
        public string TokenName;                             // "Bone Token" -- for messages

        public int StoneItemID;                              // graphic for the reward stone
        public int StoneHue;
        public int SpawnerItemID;                            // graphic for the spawner controller
        public int SpawnerHue;

        public Dictionary<Type, int> Bounties = new Dictionary<Type, int>();
        public Dictionary<Type, int> Population = new Dictionary<Type, int>();
        public List<InvasionRewardEntry> Rewards = new List<InvasionRewardEntry>();

        // Base 0.0 - 1.0 chance for each spawned creature to be converted to
        // a paragon. The spawner adds a Fame-based bonus on top of this so
        // top-tier bosses (LichLord, SkeletalDragon, ...) get a meaningful
        // shot at paragon status -- with a flat rate they were drowned out
        // by the much larger low-tier populations.
        public double ParagonChance = 0.15;

        public InvasionTheme(string name, Type tokenType, string tokenName)
        {
            Name = name;
            TokenType = tokenType;
            TokenName = tokenName;

            // Sensible defaults; themes can override.
            StoneItemID = 0xEDC;
            StoneHue = 0;
            SpawnerItemID = 0x1F1C;
            SpawnerHue = 0;
        }
    }

    public class InvasionRewardEntry
    {
        public Type Type;
        public string DisplayName;
        public int Cost;

        public InvasionRewardEntry(Type type, string name, int cost)
        {
            Type = type;
            DisplayName = name;
            Cost = cost;
        }
    }
}
