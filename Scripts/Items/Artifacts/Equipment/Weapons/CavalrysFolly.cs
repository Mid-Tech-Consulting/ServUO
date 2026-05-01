using System;

namespace Server.Items
{
    public class CavalrysFolly : BladedStaff
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1115446; } } // Cavalry's Folly
		
        [Constructable]
        public CavalrysFolly()
            : base()
        {	
            Hue = 1165;
            Attributes.BonusHits = 2;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 35;
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.HitFireball = 60;
            WeaponAttributes.HitLeechMana = 60;
            WeaponAttributes.HitLeechHits = 60;
            AosElementDamages.Physical = 0;
            AosElementDamages.Fire = 100;
        }

        public CavalrysFolly(Serial serial)
            : base(serial)
        {
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}