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
    public class TailorStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Tailoring Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Leather Types
                    typeof(Leather), typeof(SpinedLeather), typeof(HornedLeather), typeof(BarbedLeather),
                    
                    // Raw Hide Types (Standard ServUO names)
                    typeof(Hides), typeof(SpinedHides), typeof(HornedHides), typeof(BarbedHides),
                    
                    // Gems & Pelts
                    typeof(ArcaneGem), typeof(TigerPelt), typeof(WhiteTigerPelt), typeof(BlackTigerPelt),
                    
                    // Raw Materials
                    typeof(Cotton), typeof(Flax), typeof(Wool), typeof(SpoolOfThread), typeof(LightYarn), typeof(DarkYarn),
                    
                    // Finished Materials
                    typeof(Cloth), typeof(BoltOfCloth), typeof(DragonTurtleScute),
                    
                    // Supplies
                    typeof(Bandage), typeof(EnhancedBandage), typeof(Bone)
                };
            }
        }

        [Constructable]
        public TailorStorageChest() : base(0xE7C) 
        {
            Name = "Tailoring Storage Chest";
        }

        public TailorStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
