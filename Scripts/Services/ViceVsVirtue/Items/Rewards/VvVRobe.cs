using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVRobe : BaseOuterTorso
	{
        public override int LabelNumber
        {
            get
            {
                if (this.Hue == ViceVsVirtueSystem.VirtueHue)
                    return 1155532;

                if (this.Hue == ViceVsVirtueSystem.ViceHue)
                    return 1155533;

                return base.LabelNumber;
            }
        }

        [Constructable]
        public VvVRobe(int hue)
            : base(0x2684, hue)
        {
            if (hue == ViceVsVirtueSystem.VirtueHue)
            {
                SkillBonuses.SetValues(0, SkillName.Magery, 10.0);
                Attributes.DefendChance = 15;
                Attributes.BonusInt = 5;
                Attributes.BonusMana = 8;
                Attributes.CastSpeed = 1;
            }
            else if (hue == ViceVsVirtueSystem.ViceHue)
            {
                SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
                Attributes.DefendChance = 15;
                Attributes.BonusDex = 5;
                Attributes.BonusStam = 8;
                Attributes.CastSpeed = 1;
            }
        }

        public VvVRobe(Serial serial)
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
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
	}
}