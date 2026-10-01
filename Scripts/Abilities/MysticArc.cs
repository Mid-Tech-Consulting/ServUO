using System;
using System.Collections.Generic;

namespace Server.Items
{
    // The thrown projectile will arc to a second target after hitting the primary target. Chaos energy will burst from the projectile at each target. 
    // This will only hit targets that are in combat with the user.
    public class MysticArc : WeaponAbility
    {
        private const int DamageAmount = 15;

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (attacker == null || defender == null)
                return;

            if (!CheckMana(attacker, true))
                return;

            BaseThrown weapon = attacker.Weapon as BaseThrown;

            if (weapon == null)
                return;

            ClearCurrentAbility(attacker);

            // Primary target takes chaos damage
            AOS.Damage(defender, attacker, DamageAmount, 0, 0, 0, 0, 0, 100);

            // Find a valid second target in combat with the thrower
            List<Mobile> targets = new List<Mobile>();
            int maxRange = weapon.MaxRange;
            IPooledEnumerable eable = defender.GetMobilesInRange(maxRange);

            foreach (Mobile m in eable)
            {
                if (m == defender || m == attacker)
                    continue;

                if (m.Map != attacker.Map || !m.Alive || m.Deleted)
                    continue;

                if (!attacker.CanSee(m) || !attacker.CanBeHarmful(m, false))
                    continue;

                if (!attacker.InRange(m, maxRange + 2) || !defender.InLOS(m))
                    continue;

                if (m.Combatant != attacker && attacker.Combatant != m)
                    continue;

                targets.Add(m);
            }

            eable.Free();

            Mobile secondTarget = targets.Count > 0 ? targets[Utility.Random(targets.Count)] : null;

            if (secondTarget != null)
            {
                defender.MovingEffect(secondTarget, weapon.ItemID, 18, 1, false, false);
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), () => ThrowAgain(attacker, defender, secondTarget, weapon));
            }
            else
            {
                // No valid second target: visual projectile returns to attacker from defender
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), () =>
                {
                    if (attacker != null && !attacker.Deleted && defender != null && !defender.Deleted && attacker.Map == defender.Map && attacker.InRange(defender, 25))
                    {
                        defender.MovingEffect(attacker, weapon.ItemID, 18, 1, false, false);
                    }
                    else if (attacker != null && !attacker.Deleted && attacker.Map != null && attacker.Map != Map.Internal)
                    {
                        Point3D loc = defender != null ? defender.Location : attacker.Location;
                        Effects.SendMovingParticles(new Entity(Serial.Zero, loc, attacker.Map), attacker, weapon.ItemID, 18, 0, false, false, weapon.Hue, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    }
                });
            }
        }

        public void ThrowAgain(Mobile attacker, Mobile defender, Mobile target, BaseThrown weapon)
        {
            if (attacker == null || attacker.Deleted || !attacker.Alive || attacker.Map == null || attacker.Map == Map.Internal)
                return;

            if (target == null || target.Deleted || !target.Alive || target.Map != attacker.Map)
                return;

            if (weapon == null || weapon.Deleted || attacker.Weapon != weapon)
                return;

            if (!attacker.InRange(target, weapon.MaxRange + 4))
                return;

            if (!attacker.CanSee(target) || !attacker.CanBeHarmful(target, false))
                return;

            attacker.DoHarmful(target);

            if (weapon.CheckHit(attacker, target))
            {
                weapon.OnHit(attacker, target, 0.0);
                AOS.Damage(target, attacker, DamageAmount, 0, 0, 0, 0, 0, 100);
            }
            else
            {
                weapon.OnMiss(attacker, target);
            }
        }

        public void ThrowAgain()
        {
        }
    }
}