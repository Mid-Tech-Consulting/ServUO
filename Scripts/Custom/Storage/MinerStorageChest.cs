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
    public class MinerStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Miner's Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Ore (This usually covers both large and small versions of these types)
                    typeof(IronOre), typeof(DullCopperOre), typeof(ShadowIronOre), typeof(CopperOre),
                    typeof(BronzeOre), typeof(GoldOre), typeof(AgapiteOre), typeof(VeriteOre),
                    typeof(ValoriteOre),

                    // Ingots
                    typeof(IronIngot), typeof(DullCopperIngot), typeof(ShadowIronIngot), typeof(CopperIngot),
                    typeof(BronzeIngot), typeof(GoldIngot), typeof(AgapiteIngot), typeof(VeriteIngot),
                    typeof(ValoriteIngot),

                    // Granite
                    typeof(Granite), typeof(DullCopperGranite), typeof(ShadowIronGranite), typeof(CopperGranite),
                    typeof(BronzeGranite), typeof(GoldGranite), typeof(AgapiteGranite), typeof(VeriteGranite),
                    typeof(ValoriteGranite),

                    // Scales
                    typeof(RedScales), typeof(YellowScales), typeof(BlackScales), typeof(GreenScales),
                    typeof(WhiteScales), typeof(BlueScales),

                    // Sand
                    typeof(Sand)
                };
            }
        }

        [Constructable]
        public MinerStorageChest() : base(0xE7C) 
        {
            Name = "Miner's Storage Chest";
        }

        public MinerStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
