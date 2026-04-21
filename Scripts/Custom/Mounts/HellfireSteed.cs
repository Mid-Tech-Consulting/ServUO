using Server.Items;
using Server.Network;
using System;

namespace Server.Mobiles
{
    [CorpseName("a hellfire steed corpse")]
    public class HellfireSteed : BaseMount
    {
        public override TimeSpan MountAbilityDelay => TimeSpan.FromMinutes(5);

        [Constructable]
        public HellfireSteed()
            : this("a Hellfire Steed")
        {
        }

        [Constructable]
        public HellfireSteed(string name)
            : base(name, 0x675, 0x3EE0, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.2)
        {
            BaseSoundID = Core.AOS ? 0xA8 : 0x16A;

            SetStr(616, 875);
            SetDex(526, 875);
            SetInt(586, 775);

            SetHits(1428, 1915);

            SetDamage(35, 50);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 55, 75);
            SetResistance(ResistanceType.Fire, 75, 95);
            SetResistance(ResistanceType.Cold, 20, 35);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 30, 55);

            SetSkill(SkillName.EvalInt, 70.4, 112.0);
            SetSkill(SkillName.Magery, 70.4, 112.0);
            SetSkill(SkillName.MagicResist, 120.3, 130.0);
            SetSkill(SkillName.Tactics, 97.6, 112.0);
            SetSkill(SkillName.Wrestling, 110.5, 119.5);

            Fame = 14000;
            Karma = -14000;

            VirtualArmor = 90;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 104.1;

            // Color roll: rare overrides only. 56% stays the natural body color.
            // 1653 is the rare body variant used for all special hues.
            int roll = Utility.Random(10000);
            if (roll < 100) { BodyValue = 1653; Hue = 0x07B7; }           // 1% Red (ultra rare)
            else if (roll < 250) { BodyValue = 1653; Hue = 1153; }         // 1.5% Luna White
            else if (roll < 400) { BodyValue = 1653; Hue = 2406; }         // 1.5% Black
            else if (roll < 1200) { BodyValue = 1653; Hue = 2048; }        // 8% existing
            else if (roll < 2000) { BodyValue = 1653; Hue = 2206; }        // 8% existing
            else if (roll < 2800) { BodyValue = 1653; Hue = 2216; }        // 8% existing
            else if (roll < 3600) { BodyValue = 1653; Hue = 2210; }        // 8% existing
            else if (roll < 4400) { BodyValue = 1653; Hue = 2228; }        // 8% existing
            // else: natural body color unchanged (56%)

            switch (Utility.Random(12))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new HorrificBeastScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new VengefulSpiritScroll()); break;
            }

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public HellfireSteed(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool AutoDispel { get { return !Controlled; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.Gems, 3);
        }

        public override int Meat => 5;
        public override int Hides => 10;
        public override HideType HideType => HideType.Horned;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        public override int GetAngerSound()
        {
            if (!Controlled)
                return 0x16A;

            return base.GetAngerSound();
        }

        public override bool DoMountAbility(int damage, Mobile attacker)
        {
            if (Rider == null || attacker == null)
                return false;

            if ((Rider.Hits - damage) < 50 && Rider.Map == attacker.Map && Rider.InRange(attacker, 18))
            {
                Rider.MovingParticles(attacker, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                Rider.PlaySound(Core.AOS ? 0x15E : 0x44B);
                PlaySound(GetAngerSound());

                Mobile rider = Rider;

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    if (attacker != null && !attacker.Deleted && rider != null && attacker.Alive && attacker.Map == rider.Map)
                    {
                        int finalDamage = Utility.RandomMinMax(15, 25);
                        attacker.Damage(finalDamage, this, false);
                    }
                });

                Rider.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042534);
                Rider.FixedParticles(0, 0, 0, 0x13A7, EffectLayer.Waist);
                Rider.PlaySound(0xA9);
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
                ControlSlots = 3;
        }
    }
}
