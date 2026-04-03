using Server;
using System;

namespace Server.Items
{
    public class EpauletteBearingTheCrestOfBlackthorn2 : Cloak
    {
        public override bool IsArtifact { get { return true; } }

        public override int LabelNumber { get { return 1123325; } } // Epaulette

        [Constructable]
        public EpauletteBearingTheCrestOfBlackthorn2()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            ItemID = 0x9985;
            SkillName[] skills = new SkillName[] { SkillName.Veterinary, SkillName.Fishing, SkillName.Cartography, SkillName.Begging, SkillName.Snooping };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.BonusMana = 10;
            Attributes.LowerManaCost = 15;
            Attributes.Luck = 100;
            Hue = 1306;

			Layer = Layer.OuterTorso;
        }       

        public EpauletteBearingTheCrestOfBlackthorn2(Serial serial)
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

                if (Layer != Layer.OuterTorso)
                {
                    if (Parent is Mobile)
                    {
                        ((Mobile)Parent).AddToBackpack(this);
                    }

                    Layer = Layer.OuterTorso;
                }
            }
        }
    }
}