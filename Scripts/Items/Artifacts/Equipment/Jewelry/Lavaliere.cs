using System;

namespace Server.Items
{
    public class Lavaliere : GoldNecklace
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114843; } } // Lavaliere
		
        [Constructable]
        public Lavaliere()
        {
            Hue = 1194;
            AbsorptionAttributes.EaterKinetic = 20;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 20;
            Resistances.Physical = 15;
            Resistances.Fire = 15;
            Resistances.Cold = 15;
            Resistances.Poison = 15;
            Resistances.Energy = 15;
            SkillBonuses.SetValues(0, SkillName.EvalInt, 15.0);
        }

        public Lavaliere(Serial serial)
            : base(serial)
        {
        }       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
