using Server;
using System;

namespace Server.Items
{
    public class FancyDressBearingTheCrestOfBlackthorn1 : FancyDress
    {
        public override bool IsArtifact { get { return true; } }
                
        [Constructable]
        public FancyDressBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.RegenMana = 4;
            Attributes.Luck = 300;
            Attributes.DefendChance = 15;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Resistances.Fire = 15;
            Hue = 2075;
        }

        public FancyDressBearingTheCrestOfBlackthorn1(Serial serial)
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