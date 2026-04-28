using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVEpaulette : Epaulette
	{
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVEpaulette()
        {
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            Attributes.WeaponSpeed = 10;
            Attributes.BonusStam = 8;
            Attributes.BonusDex = 5;
            Attributes.AttackChance = 5;
            Attributes.RegenMana = 2;
        }

        public VvVEpaulette(Serial serial)
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

    public class VvVGargishEpaulette : GargishEpaulette
    {
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVGargishEpaulette()
        {
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            Attributes.WeaponSpeed = 10;
            Attributes.BonusStam = 8;
            Attributes.BonusDex = 5;
            Attributes.AttackChance = 5;
            Attributes.RegenMana = 2;
        }

        public VvVGargishEpaulette(Serial serial)
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