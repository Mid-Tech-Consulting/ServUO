using System;

namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfDeath : BaseOuterTorso
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CloakOfDeath()
            : base(0x2FB9)
        {
            Weight = 2.0;
			Hue = 0x966;
			Attributes.AttackChance = 15;
			Attributes.DefendChance = 15;
			Attributes.SpellDamage = 15;
			Attributes.EnhancePotions = 15;
			Attributes.RegenStam = 6;
        }

        public CloakOfDeath(Serial serial)
            : base(serial)
        {
        }
		
		public override int LabelNumber {get {return 1112881;} }// Cloak of Death

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}