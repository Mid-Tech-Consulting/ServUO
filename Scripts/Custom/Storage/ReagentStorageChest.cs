/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    [Flipable(0xE7C, 0x9AB)]
    public class ReagentStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Reagent Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Standard Reagents
                    typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic), 
                    typeof(Ginseng), typeof(MandrakeRoot), typeof(Nightshade), 
                    typeof(SulfurousAsh), typeof(SpidersSilk),
                    // Necro / Rare Reagents
                    typeof(BatWing), typeof(GraveDust), typeof(DaemonBlood),
                    typeof(NoxCrystal), typeof(PigIron), typeof(Bone),
                    typeof(DragonBlood), typeof(FertileDirt), typeof(DaemonBone),
                    // Other
                    typeof(Bottle)
                };
            }
        }

        [Constructable]
        public ReagentStorageChest() : base(0xE7C) // Metal Chest Graphic
        {
            Name = "Reagent Storage Chest";
        }

        public ReagentStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
