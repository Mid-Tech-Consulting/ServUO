using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class MortalStrike : WeaponAbility
    {
        public static readonly TimeSpan PlayerDuration = TimeSpan.FromSeconds(8.0);
        public static readonly TimeSpan NPCDuration = TimeSpan.FromSeconds(14.0);

        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        private static readonly List<Mobile> m_Immune = new List<Mobile>();

        public MortalStrike()
        {
        }

        public override int BaseMana { get { return 30; } }

        // Base hit always deals 70% weapon damage; 130% case is handled in OnHit
        public override double DamageScalar { get { return 0.7; } }

        public static bool IsWounded(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static bool IsImmune(Mobile m)
        {
            return m_Immune.Contains(m);
        }

        public static void BeginWound(Mobile m, TimeSpan duration)
        {
            if (m_Table.ContainsKey(m))
                EndWound(m, true);

            Timer t = new InternalTimer(m, duration);
            m_Table[m] = t;
            t.Start();

            m.YellowHealthbar = true;
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.MortalStrike, 1075810, 1075811, duration, m));
        }

        public static void EndWound(Mobile m, bool natural = false)
        {
            if (!IsWounded(m))
                return;

            Timer t = m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            BuffInfo.RemoveBuff(m, BuffIcon.MortalStrike);

            m.YellowHealthbar = false;
            m.SendLocalizedMessage(1060208); // You are no longer mortally wounded.

            if (!m_Immune.Contains(m))
            {
                m_Immune.Add(m);

                Timer.DelayCall(TimeSpan.FromSeconds(8.0), () =>
                {
                    m_Immune.Remove(m);
                });
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            defender.PlaySound(0x1E1);
            defender.FixedParticles(0x37B9, 244, 25, 9944, 31, 0, EffectLayer.Waist);

            if (IsWounded(defender) || IsImmune(defender))
            {
                // DamageScalar already applied 70%; add 60% of original (6/7 of current) to reach 130% total
                int extra = damage * 6 / 7;
                AOS.Damage(defender, attacker, extra, 100, 0, 0, 0, 0);
            }
            else
            {
                attacker.SendLocalizedMessage(1060086); // You deliver a mortal wound!
                defender.SendLocalizedMessage(1060087); // You have been mortally wounded!

                TimeSpan duration = defender.Player ? PlayerDuration : NPCDuration;

                if (attacker.Weapon is BaseRanged)
                    duration = TimeSpan.FromSeconds(duration.TotalSeconds / 2.0);

                if (Spells.SkillMasteries.ResilienceSpell.UnderEffects(defender))
                    duration = TimeSpan.FromSeconds(duration.TotalSeconds / 2.0);

                BeginWound(defender, duration);
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndWound(m_Mobile, true);
            }
        }
    }
}
