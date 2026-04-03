using Server;
using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn2 : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem { get { return DefTailoring.CraftSystem; } }       
        public override int LabelNumber { get { return 1029863; } } // Hooded Robe
        public override bool IsArtifact { get { return true; } }        
        
        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn2()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillName[] skills = new SkillName[] { SkillName.Veterinary, SkillName.Fishing, SkillName.Cartography, SkillName.Begging, SkillName.Snooping };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.BonusMana = 10;
            Attributes.LowerManaCost = 15;
            Attributes.Luck = 100;
            Hue = 1306;
        }

        public HoodedRobeBearingTheCrestOfBlackthorn2(Serial serial)
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