using Server.Mobiles;

namespace Server.Items
{
    public class DragonHildebrandtBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public DragonHildebrandtBondedStatuette()
            : base(0xB162)
        {
            Name = "a bonded statuette of a Dragon Hildebrandt";
            Weight = 1.0;
        }

        public DragonHildebrandtBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon { get { return new DragonHildebrandt(); } }

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
