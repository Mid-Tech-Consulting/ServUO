using System;

namespace Server.Items
{
    public class AlchemistsBauble : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AlchemistsBauble()
        {
            Hue = 0x290;
            SkillBonuses.SetValues(0, SkillName.Alchemy, 20.0);
            Attributes.EnhancePotions = 35;
            Attributes.LowerRegCost = 20;
            Attributes.DefendChance = 15;
            Attributes.CastRecovery = 3;
            Attributes.RegenMana = 2;
            Resistances.Poison = 10;
        }

        public AlchemistsBauble(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070638;
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