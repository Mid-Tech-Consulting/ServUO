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
    public class HighSeasStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "High Seas Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Raw Materials & Ignition
                    typeof(BlackPowder), typeof(Potash), typeof(Charcoal), 
                    typeof(Saltpeter), typeof(Matches),

                    // Powder Charges
                    typeof(LightPowderCharge), typeof(HeavyPowderCharge),

                    // Standard Cannonballs
                    typeof(LightCannonball), typeof(HeavyCannonball),

                    // Specialized Cannonballs
                    typeof(LightFlameCannonball), typeof(HeavyFlameCannonball),
                    typeof(LightFrostCannonball), typeof(HeavyFrostCannonball),

                    // Grapeshot
                    typeof(LightGrapeshot), typeof(HeavyGrapeshot)
                };
            }
        }

        [Constructable]
        public HighSeasStorageChest() : base(0xE7C) 
        {
            Name = "High Seas Storage Chest";
        }

        public HighSeasStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
