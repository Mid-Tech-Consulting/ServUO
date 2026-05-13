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
    public class StygianAbyssStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Stygian Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Imbuing Ingredients (Core)
                    typeof(MagicalResidue), typeof(EnchantedEssence), typeof(RelicFragment),
                    typeof(ArcanicRuneStone), typeof(CrushedGlass), typeof(CrystalShards),
                    
                    // Essences
                    typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance),
                    typeof(EssenceControl), typeof(EssenceDiligence), typeof(EssenceDirection),
                    typeof(EssenceFeeling), typeof(EssenceOrder), typeof(EssencePassion),
                    typeof(EssencePersistence), typeof(EssenceSingularity),
                    
                    // Monster/Expansion Drops
                    typeof(AbyssalCloth), typeof(BottleIchor), typeof(BouraPelt),
                    typeof(ChagaMushroom), typeof(CrystallineBlackrock), typeof(DaemonClaw),
                    typeof(DelicateScales), typeof(ElvenFletching), typeof(FaeryDust),
                    typeof(Fur), typeof(GoblinBlood), typeof(LavaSerpentCrust),
                    typeof(MedusaBlood), typeof(MedusaLightScales), typeof(MedusaDarkScales),
                    typeof(PowderedIron), typeof(PrimalLichDust), typeof(RaptorTeeth),
                    typeof(ReflectiveWolfEye), typeof(SeedOfRenewal), typeof(ScouringToxin),
                    typeof(SilverSerpentVenom), typeof(SilverSnakeSkin), typeof(SlithEye),
                    typeof(SlithTongue), typeof(SpiderCarapace), typeof(ToxicVenomSac),
                    typeof(UndyingFlesh), typeof(VialOfVitriol), typeof(VileTentacles),
                    typeof(VoidCore), typeof(VoidEssence), typeof(VoidOrb)
                };
            }
        }

        [Constructable]
        public StygianAbyssStorageChest() : base(0xE7C) 
        {
            Name = "Stygian Abyss Storage Chest";
        }

        public StygianAbyssStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
