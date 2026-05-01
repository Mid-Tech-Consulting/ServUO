using System;

namespace Server.Items
{
    public class MinaxsArmor : FemaleStuddedChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MinaxsArmor()
            : base()
        {
            Hue = 0x453;	
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 8;
            Attributes.DefendChance = 5;
            Attributes.AttackChance = 5;
            ArmorAttributes.MageArmor = 1;			
        }

        public MinaxsArmor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073257;
            }
        }// Minax's Armor - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 25;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
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

    public class KeeoneansChainMail : ChainChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public KeeoneansChainMail()
            : base()
        {
            Hue = 0x84E;	
            Attributes.RegenHits = 3;
            Attributes.NightSight = 1;
            Attributes.BonusStr = 5;
            ArmorAttributes.MageArmor = 1;
            SkillBonuses.SetValues(0, SkillName.Blacksmith, 20.0);			
        }

        public KeeoneansChainMail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073264;
            }
        }// Keeonean's Chain Mail - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
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

    public class VesperOrderShield : OrderShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VesperOrderShield()
            : base()
        {
            Hue = 0x835;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 250;
            Attributes.CastSpeed = -1;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            AbsorptionAttributes.EaterDamage = 5;
            SkillBonuses.SetValues(0, SkillName.Parry, 20.0);
        }

        public VesperOrderShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073258;
            }
        }// Order Shield - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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

    public class VesperChaosShield : ChaosShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VesperChaosShield()
            : base()
        {
            Hue = 0xFA;
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 5;
            Attributes.SpellDamage = 15;
            Attributes.SpellChanneling = 1;
            Attributes.CastRecovery = 2;	
            Attributes.CastSpeed = 1;				
            ArmorAttributes.SelfRepair = 1;
            SkillBonuses.SetValues(0, SkillName.Discordance, 20.0);
        }

        public VesperChaosShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073259;
            }
        }// Chaos Shield - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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