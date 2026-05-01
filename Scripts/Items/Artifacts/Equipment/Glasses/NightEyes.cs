using System;

namespace Server.Items
{
    public class NightEyes : Glasses
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114785; } } // Night Eyes
		
        private static readonly SkillName[] m_RandomSkills =
        {
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.Necromancy,
            SkillName.SpiritSpeak,
            SkillName.Focus,
            SkillName.Meditation,
            SkillName.Cartography,
            SkillName.Mining,
            SkillName.Blacksmith,
            SkillName.Tailoring,
            SkillName.Fishing,
        };

        [Constructable]
        public NightEyes()
            : base()
        {
            Hue = 26;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 3;
            Attributes.LowerRegCost = 20;
            Attributes.LowerManaCost = 8;
            Attributes.Luck = 150;
            Attributes.SpellDamage = 10;
            SkillBonuses.SetValues(0, m_RandomSkills[Utility.Random(m_RandomSkills.Length)], 20.0);
        }

        public NightEyes(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}