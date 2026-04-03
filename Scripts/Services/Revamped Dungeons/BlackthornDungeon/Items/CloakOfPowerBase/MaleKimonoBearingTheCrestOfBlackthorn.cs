using Server;
using System;

namespace Server.Items
{
    public class MaleKimonoBearingTheCrestOfBlackthorn4 : MaleKimono
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public MaleKimonoBearingTheCrestOfBlackthorn4()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.Luck = 100;
            Attributes.BonusStr = 10;
            Attributes.BonusDex = 10;
            Attributes.BonusInt = 10;
            Attributes.SpellDamage = 30;
            Hue = 2107;
        }

        public MaleKimonoBearingTheCrestOfBlackthorn4(Serial serial)
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