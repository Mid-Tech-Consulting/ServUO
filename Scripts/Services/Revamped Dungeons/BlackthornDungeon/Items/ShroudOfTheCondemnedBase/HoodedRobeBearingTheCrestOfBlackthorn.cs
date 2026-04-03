using Server;
using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn1 : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem { get { return DefTailoring.CraftSystem; } }       
        public override int LabelNumber { get { return 1029863; } } // Hooded Robe
        public override bool IsArtifact { get { return true; } }        
        
        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn1()
            : base(0x2683)
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

        public HoodedRobeBearingTheCrestOfBlackthorn1(Serial serial)
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