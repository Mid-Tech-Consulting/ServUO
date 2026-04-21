using Server.Mobiles;

namespace Server.Items
{
    public class WildfireOstardBondedStatuette : BaseImprisonedMobile
    {
        [Constructable]
        public WildfireOstardBondedStatuette()
            : base(0x20DA)
        {
            Name = "a bonded statuette of a Wildfire Ostard";
            Weight = 1.0;
            Hue = 1175;
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
