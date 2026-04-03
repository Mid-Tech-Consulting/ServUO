using Server;
using System;

namespace Server.Items
{
    public class GildedDressBearingTheCrestOfBlackthorn7 : GildedDress
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public GildedDressBearingTheCrestOfBlackthorn7()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillName[] skills = new SkillName[] { SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.BonusMana = 10;
            Attributes.BonusInt = 10;
            Attributes.BonusHits = 5;
            Attributes.Luck = 100;
            Hue = 1194;            
        }

        public GildedDressBearingTheCrestOfBlackthorn7(Serial serial)
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