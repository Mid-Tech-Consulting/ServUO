using Server;
using System;

namespace Server.Items
{
    public class LeatherNinjaBeltBearingTheCrestOfBlackthorn1 : LeatherNinjaBelt
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public LeatherNinjaBeltBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 10;
            Attributes.Luck = 100;
            SAAbsorptionAttributes.EaterKinetic = 15;
            StrRequirement = 10;
            Hue = 2527;

            switch (Utility.Random(7))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Magery, 10.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Mysticism, 10.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Bushido, 10.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10.0); break;
                case 6: SkillBonuses.SetValues(0, SkillName.Spellweaving, 10.0); break;
            }
        }   

        public LeatherNinjaBeltBearingTheCrestOfBlackthorn1(Serial serial)
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