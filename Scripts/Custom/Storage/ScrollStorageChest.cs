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
    public class ScrollStorageChest : VirtualStorageChest
    {
        // Matches the "Reagent Stockpile" naming convention
        public override string ChestTitle { get { return "Scroll Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                // On your shard, Magery, Necromancy, Mysticism, and Spellweaving scrolls 
                // all inherit from 'SpellScroll', so this single entry covers them all.
                return new Type[] 
                { 
                    typeof(SpellScroll) 
                };
            }
        }

        [Constructable]
        public ScrollStorageChest() : base(0xE7C) // Metal Chest Graphic
        {
            Name = "Scroll Storage Chest";
            // Removed custom Hue to match the default metal and red edging look
        }

        public ScrollStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
