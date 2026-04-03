using Server;
using System;

namespace Server.Items
{
    public class PlainDressBearingTheCrestOfBlackthorn2 : PlainDress
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public PlainDressBearingTheCrestOfBlackthorn2()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillName[] skills = new SkillName[] { SkillName.Veterinary, SkillName.Fishing, SkillName.Cartography, SkillName.Begging, SkillName.Snooping };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.BonusMana = 10;
            Attributes.LowerManaCost = 15;
            Attributes.Luck = 100;
            Hue = 2075;
        }

        public PlainDressBearingTheCrestOfBlackthorn2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
			
			if (version == 0)
            {
                MaxHitPoints = 0;
                HitPoints = 0;
            }
        }
    }
}