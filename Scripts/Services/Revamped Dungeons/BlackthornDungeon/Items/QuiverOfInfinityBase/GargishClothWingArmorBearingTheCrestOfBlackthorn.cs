using Server;
using System;

namespace Server.Items
{
    public class GargishClothWingArmorBearingTheCrestOfBlackthorn : GargishClothWingArmor
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishClothWingArmorBearingTheCrestOfBlackthorn()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillName[] skills = new SkillName[] { SkillName.RemoveTrap, SkillName.Lockpicking, SkillName.Stealing, SkillName.Tracking, SkillName.Herding };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.Luck = 200;
            Attributes.AttackChance = 10;
            Attributes.SpellDamage = 10;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 10;
            Attributes.RegenHits = 2;
            Attributes.RegenMana = 2;
            Resistances.Fire = 15;
            this.Hue = 1766;
        }

        public GargishClothWingArmorBearingTheCrestOfBlackthorn(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}