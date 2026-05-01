using System;

namespace Server.Items
{
    public class StoneSlithClaw : Cyclone
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StoneSlithClaw()
        {	
            Hue = 1150;
            Slayer = SlayerName.ReptilianDeath;
            WeaponAttributes.HitHarm = 70;
            WeaponAttributes.HitLowerDefend = 50;
            WeaponAttributes.HitColdArea = 80;
            WeaponAttributes.HitLeechMana = 80;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 45;
            AosElementDamages.Physical = 0;
            AosElementDamages.Cold = 100;
        }

        public StoneSlithClaw(Serial serial)
            : base(serial)
        {
        }
        
        public override int LabelNumber { get{return 1112393;} }// Stone Slith Claw

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
