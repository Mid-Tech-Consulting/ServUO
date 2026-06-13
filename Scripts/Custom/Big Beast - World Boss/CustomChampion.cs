using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
    [CorpseName("a champion corpse")]
    public class CustomChampion : BaseChampion
    {
        private string m_ThemeName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string ThemeName
        {
            get { return m_ThemeName; }
            set 
            { 
                m_ThemeName = value; 
                ApplyTheme(Server.Custom.Events.CustomChampThemes.Find(m_ThemeName));
            }
        }

        public override double WeaponAbilityChance { get { return 0.9; } }
        public override WeaponAbility GetWeaponAbility() { return WeaponAbility.Dismount; }
        public override bool AlwaysMurderer { get { return false; } }
        public override bool BleedImmune { get { return true; } }

        public override ChampionSkullType SkullType 
        { 
            get 
            { 
                var theme = Server.Custom.Events.CustomChampThemes.Find(m_ThemeName);
                return theme != null ? theme.SkullType : ChampionSkullType.None; 
            } 
        }

        public override Type[] UniqueList 
        { 
            get 
            { 
                var theme = Server.Custom.Events.CustomChampThemes.Find(m_ThemeName);
                return theme != null ? theme.UniqueList : new Type[0]; 
            } 
        }

        public override Type[] SharedList 
        { 
            get 
            { 
                var theme = Server.Custom.Events.CustomChampThemes.Find(m_ThemeName);
                return theme != null ? theme.SharedList : new Type[0]; 
            } 
        }

        public override Type[] DecorativeList 
        { 
            get 
            { 
                var theme = Server.Custom.Events.CustomChampThemes.Find(m_ThemeName);
                return theme != null ? theme.DecorativeList : new Type[0]; 
            } 
        }

        public override MonsterStatuetteType[] StatueTypes { get { return new MonsterStatuetteType[0]; } }
        public override bool DropsThemedArtifacts { get { return false; } }

        [Constructable]
        public CustomChampion() : base(AIType.AI_Mage, FightMode.Closest)
        {
            // Default to Big Beast theme if not specified
            ThemeName = "Big Beast";
        }

        public override void OnChampPopped(ChampionSpawn spawn)
        {
            base.OnChampPopped(spawn);
            
            var theme = Server.Custom.Events.CustomChampThemes.Find(spawn.Type);
            if (theme != null)
                ThemeName = theme.Name;
        }

        public void ApplyTheme(Server.Custom.Events.CustomChampTheme theme)
        {
            if (theme == null)
                return;

            Name = theme.Name;
            Title = theme.Title;
            Hue = theme.Hue;
            Body = theme.Body;

            SetStr(theme.Str);
            SetDex(theme.Dex);
            SetInt(theme.Int);

            SetHits(theme.Hits);
            SetStam(theme.Stam);
            SetMana(theme.Mana);

            SetDamage(theme.MinDamage, theme.MaxDamage);

            SetResistance(ResistanceType.Physical, theme.PhysicalResist);
            SetResistance(ResistanceType.Fire, theme.FireResist);
            SetResistance(ResistanceType.Cold, theme.ColdResist);
            SetResistance(ResistanceType.Poison, theme.PoisonResist);
            SetResistance(ResistanceType.Energy, theme.EnergyResist);

            SetSkill(SkillName.EvalInt, 120.1, 130.0);
            SetSkill(SkillName.Magery, 130.1, 140.0);
            SetSkill(SkillName.MagicResist, 165.9);
            SetSkill(SkillName.Tactics, 118.0);
            SetSkill(SkillName.Wrestling, 125.0);

            Fame = 99999;
            Karma = -99999;
            VirtualArmor = 90;
            ControlSlots = 3;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.70 > Utility.RandomDouble()) // 70% chance when hit
            {
                if (attacker != null && attacker.Alive && !attacker.Deleted && this.CanBeHarmful(attacker))
                {
                    this.DoHarmful(attacker);

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

            foreach (Mobile m in this.GetMobilesInRange(5))
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

            if (0.5 >= Utility.RandomDouble())
                DrainLife();
        }

        public override int GetAngerSound() { return 357; }
        public override int GetAttackSound() { return 357; }

        public CustomChampion(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_ThemeName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_ThemeName = reader.ReadString();

            // Reapply theme attributes after reload
            ApplyTheme(Server.Custom.Events.CustomChampThemes.Find(m_ThemeName));
        }
    }
}
