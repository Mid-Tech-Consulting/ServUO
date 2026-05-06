using Server;
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.VvV
{
	public enum SteedType
	{
		Ostard = 0,
		WarHorse = 1,
		MinaxWarHorse = 2,
		ShadowlordsWarHorse = 3,
		CouncilOfMagesWarHorse = 4,
		TrueBritanniansWarHorse = 5
	}

    public class VvVSteedStatuette : BaseImprisonedMobile
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public SteedType SteedType { get; set; }

        public override BaseCreature Summon
        {
            get
            {
                switch (this.SteedType)
                {
                    default:
                    case SteedType.Ostard: return new VvVMount("a war ostard", 0xDA, 0x3EA4, this.Hue);
                    case SteedType.WarHorse: return new VvVMount("a war horse", 0xE2, 0x3EA0, this.Hue);
                    case SteedType.MinaxWarHorse: return new VvVMount("a war horse", 0x78, 0x3EAF, 0);
                    case SteedType.ShadowlordsWarHorse: return new VvVMount("a war horse", 0x79, 0x3EB0, 0);
                    case SteedType.CouncilOfMagesWarHorse: return new VvVMount("a war horse", 0x77, 0x3EB1, 0);
                    case SteedType.TrueBritanniansWarHorse: return new VvVMount("a war horse", 0x76, 0x3EB2, 0);
                }
            }
        }

		[Constructable]
		public VvVSteedStatuette(SteedType mounttype, int hue)
            : base(GetStatuetteItemID(mounttype))
		{
            Hue = hue;
            SteedType = mounttype;
		}

		private static int GetStatuetteItemID(SteedType type)
		{
			switch (type)
			{
				default:
				case SteedType.WarHorse:
				case SteedType.MinaxWarHorse:
				case SteedType.ShadowlordsWarHorse:
				case SteedType.CouncilOfMagesWarHorse:
				case SteedType.TrueBritanniansWarHorse:
					return 8484;
				case SteedType.Ostard: return 8501;
			}
		}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!ViceVsVirtueSystem.IsVvV(m))
            {
                m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                return;
            }

            base.OnDoubleClick(m);
        }
		
		public VvVSteedStatuette(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)SteedType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			SteedType = (SteedType)reader.ReadInt();
		}
	}
	
	public class VvVMount : BaseMount
	{
		public VvVMount(string name, int id, int itemid, int hue)
			: base(name, id, itemid, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.4, .2)
		{
			Hue = hue;

            if(id == 0xDA)
                BaseSoundID = 0x275;
            else
                BaseSoundID = 0xA8;

			this.InitStats(Utility.Random(300, 100), 125, 60);

            SetStr(400);
            SetDex(125);
            SetInt(51, 55);

            SetHits(240);
            SetMana(0);

            SetDamage(5, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 29.3, 44.0);
            SetSkill(SkillName.Wrestling, 29.3, 44.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
		}

        public override bool CanTransfer(Mobile m)
        {
            if (ControlMaster != null && ControlMaster.NetState != null)
                ControlMaster.SendLocalizedMessage(1155547); // Pets obtained from VvV are non-transferable.

            return false;
        }

        public override bool CanFriend(Mobile m)
        {
            if (ControlMaster != null && ControlMaster.NetState != null)
                ControlMaster.SendLocalizedMessage(1155548); // You may not add friends to a VvV War Steed.

            return false;
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }

        public override int Hides
        {
            get
            {
                return 10;
            }
        }

        public override FoodType FavoriteFood
        {
            get
            {
                if (Body == 0xDA)
                {
                    return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.FruitsAndVegies;
                }
                else
                {
                    return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
                }
            }
        }

		public VvVMount(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if (version == 0)
			{
				reader.ReadInt();      // legacy _Readiness
				reader.ReadDateTime(); // legacy NextReadinessAtrophy
			}
		}
	}
}
