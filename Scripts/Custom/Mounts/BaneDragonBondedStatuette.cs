using Server.Mobiles;

namespace Server.Items
{
    public class BaneDragonBondedStatuette : BaseImprisonedMobile
    {
        [Constructable]
        public BaneDragonBondedStatuette()
            : base(0x20E1)
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

                // Small chance to pop a rare-colored Bane Dragon when summoned.
                if (Utility.Random(250) == 0) bd.Hue = 2048;
                else if (Utility.Random(500) == 0) bd.Hue = 2206;
                else if (Utility.Random(100) == 0) bd.Hue = 2216;
                else if (Utility.Random(1000) == 0) bd.Hue = 2210;
                else if (Utility.Random(1000) == 0) bd.Hue = 2228;

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
