using Server;
using System;

namespace Server.Items
{
    public class GargishEpauletteBearingTheCrestOfBlackthorn7 : Cloak
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int LabelNumber { get { return 1123326; } } // Gargish Epaulette

        [Constructable]
        public GargishEpauletteBearingTheCrestOfBlackthorn7()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            ItemID = 0x9986;
            SkillName[] skills = new SkillName[] { SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation };
            SkillBonuses.SetValues(0, skills[Utility.Random(skills.Length)], 10.0);
            Attributes.BonusMana = 10;
            Attributes.BonusInt = 10;
            Attributes.BonusHits = 5;
            Attributes.Luck = 100;
            Hue = 1194;
			
			Layer = Layer.OuterTorso;
        }

        public GargishEpauletteBearingTheCrestOfBlackthorn7(Serial serial)
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