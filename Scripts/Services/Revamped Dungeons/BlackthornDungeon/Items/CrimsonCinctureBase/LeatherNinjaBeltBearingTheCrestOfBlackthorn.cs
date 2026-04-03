using Server;
using System;

namespace Server.Items
{
    public class LeatherNinjaBeltBearingTheCrestOfBlackthorn2 : LeatherNinjaBelt
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public LeatherNinjaBeltBearingTheCrestOfBlackthorn2()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
            Attributes.Luck = 100;
            SAAbsorptionAttributes.EaterKinetic = 15;
            StrRequirement = 10;
            Hue = 1157;

            switch (Utility.Random(6))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Swords, 10.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Macing, 10.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Fencing, 10.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Archery, 10.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Wrestling, 10.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Throwing, 10.0); break;
            }
        }   

        public LeatherNinjaBeltBearingTheCrestOfBlackthorn2(Serial serial)
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