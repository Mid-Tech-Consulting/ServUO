using System;

namespace Server.Items
{
    public class BowOfTheJukaKing : Bow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BowOfTheJukaKing()
        {
            Hue = 0x460;
            WeaponAttributes.HitMagicArrow = 50;
            WeaponAttributes.HitColdArea = 80;
            Slayer = SlayerName.ReptilianDeath;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 40;
            Attributes.WeaponSpeed = 30;
            AosElementDamages.Physical = 0;
            AosElementDamages.Cold = 100;
        }

        public BowOfTheJukaKing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070636;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
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

            int version = reader.ReadInt();
        }
    }
}