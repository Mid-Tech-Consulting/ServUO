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
    public class MondainStorageChest : VirtualStorageChest
    {
        public override string ChestTitle { get { return "Mondain's Stockpile"; } }

        public override Type[] AllowedTypes
        {
            get
            {
                return new Type[] 
                { 
                    // Standard and ML Gems
                    typeof(Citrine), typeof(EcruCitrine), typeof(Emerald), typeof(PerfectEmerald),
                    typeof(Tourmaline), typeof(Turquoise), typeof(Diamond), typeof(BlueDiamond),
                    typeof(Sapphire), typeof(StarSapphire), typeof(DarkSapphire), typeof(Ruby),
                    typeof(FireRuby), typeof(Amber), typeof(BrilliantAmber), typeof(Amethyst),
                    typeof(ArcaneGem), typeof(WhitePearl),

                    // ML Harvesting Specials
                    typeof(BarkFragment), typeof(LuminescentFungi), typeof(ParasiticPlant), typeof(SwitchItem),

                    // Peerless Reagents
                    typeof(Corruption), typeof(Taint), typeof(Blight), typeof(Putrefaction),
                    typeof(Muculent), typeof(Scourge), typeof(DaemonBone), typeof(DiseasedBark)
                };
            }
        }

        [Constructable]
        public MondainStorageChest() : base(0xE7C) 
        {
            Name = "Mondain's Legacy Storage Chest";
        }

        public MondainStorageChest(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }
}
