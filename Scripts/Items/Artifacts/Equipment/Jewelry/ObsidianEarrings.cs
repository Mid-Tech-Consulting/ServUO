using System;

namespace Server.Items
{
    public class ObsidianEarrings : GargishEarrings
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113820; } } // Obsidian Earrings

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

        [Constructable]
        public ObsidianEarrings()
        {
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            Attributes.RegenStam = 2;
            Attributes.SpellDamage = 8;
            Attributes.LowerRegCost = 20;
            Attributes.Luck = 150;
            AbsorptionAttributes.CastingFocus = 4;
            SkillBonuses.SetValues(0, SkillName.Magery, 20.0);
        }

        public ObsidianEarrings(Serial serial)
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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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
