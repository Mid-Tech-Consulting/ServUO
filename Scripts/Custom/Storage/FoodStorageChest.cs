/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server.Items;

namespace Server.Items
{
    [Flipable(0xE7C, 0x9AB)]
    public class FoodStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Food Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Meats
                    typeof(RawRibs), typeof(Ribs), typeof(RawLambLeg), typeof(LambLeg),
                    typeof(RawBird), typeof(CookedBird), typeof(RawFishSteak), typeof(FishSteak),
                    typeof(ChickenLeg), typeof(RawChickenLeg), typeof(Bacon), typeof(SlabOfBacon),
                    typeof(Sausage), typeof(Ham), typeof(RawRotwormMeat),

                    // Bakery & Dairy
                    typeof(FrenchBread), typeof(Muffins), typeof(CheeseWheel), typeof(CheeseWedge),
                    typeof(Eggs), typeof(JarHoney),

                    // Produce
                    typeof(Pumpkin), typeof(Plum), typeof(Banana),
                    typeof(Bananas), typeof(Pear), typeof(Peach), typeof(Lemon),
                    typeof(Lime), typeof(Grapes), typeof(Apple), typeof(TribalBerry),
                    typeof(Watermelon), typeof(SplitCoconut), typeof(Squash), typeof(EarOfCorn),
                    typeof(Onion), typeof(Turnip), typeof(Carrot),

                    // Chocolatiering & Ingredients
                    typeof(SackOfSugar), typeof(SackFlour), typeof(Dough), typeof(Vanilla),
                    typeof(CocoaButter), typeof(CocoaLiquor), typeof(CocoaPulp), typeof(DarkChocolate),
                    typeof(MilkChocolate), typeof(WhiteChocolate), typeof(SweetCocoaButter),
                    typeof(DarkTruffle), typeof(FreshGinger),

                    // Standard and High Seas Fish
                    typeof(Fish),
                    typeof(BaseHighseasFish), 

                    // Crabs and Lobsters
                    typeof(BaseCrabAndLobster),

                    // High Seas Fish Steaks
                    typeof(AutumnDragonfishSteak), typeof(BullFishSteak), typeof(CrystalFishSteak),
                    typeof(FairySalmonSteak), typeof(FireFishSteak), typeof(GiantKoiSteak),
                    typeof(GreatBarracudaSteak), typeof(HolyMackerelSteak), typeof(LavaFishSteak),
                    typeof(ReaperFishSteak), typeof(SummerDragonfishSteak), typeof(UnicornFishSteak),
                    typeof(YellowtailBarracudaSteak),

                    // Rare Seasonings
                    typeof(MentoSeasoning), typeof(SamuelsSecretSauce)
                };
            }
        }

        [Constructable]
        public FoodStorageChest() : base(0xE7C) 
        {
            Name = "Food Storage Chest";
        }

        public FoodStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
