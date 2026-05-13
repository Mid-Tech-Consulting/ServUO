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
    public class LogStorageChest : VirtualStorageChest
    {
        // Matches your "Stockpile" naming convention
        public override string ChestTitle { get { return "Wood Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Standard and Special Logs
                    typeof(Log), typeof(OakLog), typeof(AshLog), 
                    typeof(YewLog), typeof(HeartwoodLog), typeof(BloodwoodLog), 
                    typeof(FrostwoodLog),
                    
                    // Standard and Special Boards
                    typeof(Board), typeof(OakBoard), typeof(AshBoard), 
                    typeof(YewBoard), typeof(HeartwoodBoard), typeof(BloodwoodBoard), 
                    typeof(FrostwoodBoard),

                    // Fletching and Crafting Items
                    typeof(Bolt), typeof(Arrow), typeof(Shaft), 
                    typeof(Feather), typeof(BlankScroll)
                };
            }
        }

        [Constructable]
        public LogStorageChest() : base(0xE7C) // Metal Chest Graphic to match Reagent/Scroll chests
        {
            Name = "Log & Board Storage Chest";
            // Uses default metal color for consistency with your updated Reagent chest
        }

        public LogStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
