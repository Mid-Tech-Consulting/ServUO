using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class HalloweenDragonBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public HalloweenDragonBondedStatuette()
            : base(0x20D6) // Classic Dragon Statuette
        {
            Name = "a bonded statuette of a Halloween Dragon";
            Weight = 1.0;
            Hue = 1161; // Glowing Blaze Orange
            LootType = LootType.Blessed;
        }

        public HalloweenDragonBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon
        {
            get
            {
                return new HalloweenDragon();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FFA500>3-Slot Trainable Pet Dragon with Innate Healing</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FFFFFF>Requires: 100.0 Animal Taming & 100.0 Animal Lore</BASEFONT>");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
