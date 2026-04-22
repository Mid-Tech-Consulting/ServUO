using Server.Mobiles;

namespace Server.Items
{
    public class WildfireOstardBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public WildfireOstardBondedStatuette()
            : base(0x2135)
        {
            Name = "a bonded statuette of a Wildfire Ostard";
            Weight = 1.0;
            Hue = 0x0AC6;
        }

        public WildfireOstardBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon { get { return new WildfireOstard(); } }

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
