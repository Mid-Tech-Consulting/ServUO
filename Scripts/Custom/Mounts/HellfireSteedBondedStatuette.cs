using Server.Mobiles;

namespace Server.Items
{
    public class HellfireSteedBondedStatuette : BaseImprisonedMobile
    {
        [Constructable]
        public HellfireSteedBondedStatuette()
            : base(0xB165)
        {
            Name = "a bonded statuette of a Hellfire Steed";
            Weight = 1.0;
        }

        public HellfireSteedBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon { get { return new HellfireSteed(); } }

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
