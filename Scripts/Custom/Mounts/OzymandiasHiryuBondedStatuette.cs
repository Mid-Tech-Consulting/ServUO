using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class OzymandiasHiryuBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public OzymandiasHiryuBondedStatuette()
            : base(0x276A)
        {
            Name = "Ozymandias' Hiryu";
            Weight = 1.0;
            Hue = 0x09C4;
            LootType = LootType.Blessed;
        }

        public OzymandiasHiryuBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        // No taming/lore gate — anyone can pop this statuette.
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.SendGump(new ConfirmBreakCrystalGump(this));
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Shard Bound"); // ~1_NOTHING~
        }

        public override BaseCreature Summon
        {
            get
            {
                OzymandiasHiryu hiryu = new OzymandiasHiryu();

                // Rare color roll matches HellfireSteed / BaneDragon statuettes.
                // 55% keeps the natural 0x09C4 from the constructor.
                int roll = Utility.Random(10000);
                if (roll < 100) hiryu.Hue = 0x000B;                 // 1% Sigil (ultra rare)
                else if (roll < 200) hiryu.Hue = 0x07B7;            // 1% Red (ultra rare)
                else if (roll < 350) hiryu.Hue = 1153;              // 1.5% Luna White
                else if (roll < 500) hiryu.Hue = 2406;              // 1.5% Black
                else if (roll < 1300) hiryu.Hue = 2048;             // 8%
                else if (roll < 2100) hiryu.Hue = 2206;             // 8%
                else if (roll < 2900) hiryu.Hue = 2216;             // 8%
                else if (roll < 3700) hiryu.Hue = 2210;             // 8%
                else if (roll < 4500) hiryu.Hue = 2228;             // 8%
                // else: natural 0x09C4 (55%)

                return hiryu;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
