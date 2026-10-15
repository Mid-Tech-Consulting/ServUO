using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Events.TurkeyEvent
{
    [CorpseName("a giant turkey corpse")]
    public class EventGiantTurkey : BaseCreature
    {
        public override double WeaponAbilityChance => 0.9;
        public override WeaponAbility GetWeaponAbility() => WeaponAbility.Dismount;
        public override bool BleedImmune => true;
        public override bool AlwaysMurderer => true;

        [Constructable]
        public EventGiantTurkey() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant turkey";
            Title = "the Forest Terror";
            Body = 1026;
            BaseSoundID = 0x66A;

            // Same stats as Big Beast
            SetStr(1500);
            SetDex(1500);
            SetInt(1500);

            SetHits(320000);
            SetStam(750);
            SetMana(1200);

            SetDamage(35, 45);

            SetResistance(ResistanceType.Physical, 80);
            SetResistance(ResistanceType.Fire, 65);
            SetResistance(ResistanceType.Cold, 85);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 75);

            SetSkill(SkillName.EvalInt, 120.1, 130.0);
            SetSkill(SkillName.Magery, 130.1, 140.0);
            SetSkill(SkillName.MagicResist, 165.9);
            SetSkill(SkillName.Tactics, 118.0);
            SetSkill(SkillName.Wrestling, 125.0);

            Fame = 35000;
            Karma = -35000;
            VirtualArmor = 90;
        }

        public override int Feathers => 250;
        public override int Meat => 25;
        public override MeatType MeatType => MeatType.Bird;
        public override FoodType FavoriteFood => FoodType.GrainsAndHay;

        public override int GetIdleSound() => 0x66A;
        public override int GetAngerSound() => 0x66A;
        public override int GetHurtSound() => 0x66B;
        public override int GetDeathSound() => 0x66B;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
        }

        // Specials matching Big Beast
        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Utility.RandomDouble() < 0.70) // 70% chance when hit
            {
                if (attacker != null && attacker.Alive && !attacker.Deleted && CanBeHarmful(attacker))
                {
                    DoHarmful(attacker);

                    switch (Utility.Random(4))
                    {
                        case 0: // Magic Arrow
                            attacker.Damage(Utility.RandomMinMax(15, 25), this);
                            attacker.FixedParticles(0x36E4, 5, 21, 10915, EffectLayer.Waist);
                            attacker.PlaySound(0x1E5);
                            attacker.SendMessage("You are hit by a magic arrow counterattack!");
                            break;
                        case 1: // Fireball
                            attacker.Damage(Utility.RandomMinMax(25, 35), this);
                            attacker.FixedParticles(0x36E4, 7, 15, 9502, 5, 0, EffectLayer.Waist);
                            attacker.PlaySound(0x15E);
                            attacker.SendMessage("You are hit by a fireball counterattack!");
                            break;
                        case 2: // Lightning
                            attacker.Damage(Utility.RandomMinMax(30, 40), this);
                            attacker.BoltEffect(0);
                            attacker.PlaySound(0x29);
                            attacker.SendMessage("You are hit by a lightning bolt counterattack!");
                            break;
                        case 3: // Mind Blast
                            attacker.Damage(Utility.RandomMinMax(35, 50), this);
                            attacker.FixedParticles(0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head);
                            attacker.PlaySound(0x286);
                            attacker.SendMessage("You are hit by a mind blast counterattack!");
                            break;
                    }
                }
            }
        }

        public void DrainLife()
        {
            ArrayList list = new ArrayList();

            foreach (Mobile m in GetMobilesInRange(5))
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            foreach (Mobile m in list)
            {
                DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                int toDrain = Utility.RandomMinMax(20, 40);
                Hits += toDrain;
                m.Damage(toDrain, this);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= 0.50)
            {
                DrainLife();
            }
        }

        public override bool OnBeforeDeath()
        {
            // Award 200 Turkey Feathers to each attacking player's backpack
            DistributeFeathers();

            // Notify manager for respawn
            TurkeyEvent.OnTurkeyKilled(this);

            return base.OnBeforeDeath();
        }

        private void DistributeFeathers()
        {
            int featherAmount = TurkeyEvent.FeathersPerKill;

            List<DamageStore> rights = GetLootingRights();
            HashSet<PlayerMobile> rewarded = new HashSet<PlayerMobile>();

            for (int i = 0; i < rights.Count; i++)
            {
                DamageStore ds = rights[i];
                if (ds != null && ds.m_Mobile is PlayerMobile pm && !pm.Deleted && ds.m_Damage > 0 && rewarded.Add(pm))
                {
                    if (pm.Backpack != null)
                    {
                        pm.AddToBackpack(new TurkeyFeather(featherAmount));
                        pm.PlaySound(0x2E6);
                        pm.SendMessage(0x35, "You harvested {0} Turkey Feathers for helping defeat the Giant Turkey!", featherAmount);
                    }
                }
            }

            if (rewarded.Count == 0)
            {
                // Fallback: Drop on corpse if no player attackers were found
                Corpse c = this.Corpse as Corpse;
                if (c != null)
                {
                    c.DropItem(new TurkeyFeather(featherAmount));
                }
            }
        }

        public EventGiantTurkey(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Re-register with event manager if alive
            TurkeyEvent.RegisterAlive(this);
        }
    }
}
