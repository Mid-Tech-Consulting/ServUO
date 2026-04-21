using Server.Mobiles;

namespace Server.Items
{
    public class BaneDragonBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public BaneDragonBondedStatuette()
            : base(0x2619)
        {
            Name = "a bonded statuette of a Bane Dragon";
            Weight = 1.0;
            Hue = 1175;
        }

        public BaneDragonBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon
        {
            get
            {
                BaneDragon bd = new BaneDragon();

                // Color roll: rare overrides only. 86% stays the natural 1175 from BaneDragon ctor.
                int roll = Utility.Random(10000);
                if (roll < 100) bd.Hue = 0x07B7;                   // 1% Red (ultra rare)
                else if (roll < 250) bd.Hue = 1153;                 // 1.5% Luna White
                else if (roll < 400) bd.Hue = 2406;                 // 1.5% Black
                else if (roll < 600) bd.Hue = 2048;                 // 2% existing
                else if (roll < 800) bd.Hue = 2206;                 // 2% existing
                else if (roll < 1000) bd.Hue = 2216;                // 2% existing
                else if (roll < 1200) bd.Hue = 2210;                // 2% existing
                else if (roll < 1400) bd.Hue = 2228;                // 2% existing
                // else: natural 1175 from BaneDragon constructor (86%)

                return bd;
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
