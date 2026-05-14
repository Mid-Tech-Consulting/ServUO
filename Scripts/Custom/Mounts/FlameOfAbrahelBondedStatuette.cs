using Server.Mobiles;

namespace Server.Items
{
    public class FlameOfAbrahelBondedStatuette : BaseDragonEggStatuette
    {
        [Constructable]
        public FlameOfAbrahelBondedStatuette()
            : base(0x20DD) // Standard horse statuette art (same one the Ethereal Horse uses)
        {
            Name = "a bonded statuette of Flame Of Abrahel";
            Weight = 1.0;
            Hue = 0x04EB; // Match the live mount's fiery orange-red tint
        }

        public FlameOfAbrahelBondedStatuette(Serial serial)
            : base(serial)
        {
        }

        public override BaseCreature Summon { get { return new FlameOfAbrahel(); } }

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
