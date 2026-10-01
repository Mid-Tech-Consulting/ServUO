using System;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseThrown : BaseRanged
    {
        private Mobile m_Thrower;
        private Mobile m_Target;
        private Point3D m_KillSave;

        public BaseThrown(int itemID)
            : base(itemID)
        {
        }

        public BaseThrown(Serial serial)
            : base(serial)
        {
        }

        public abstract int MinThrowRange { get; }
        public virtual int MaxThrowRange
        {
            get
            {
                return MinThrowRange + 3;
            }
        }

        public override int DefMaxRange
        {
            get
            {
                int baseRange = MaxThrowRange;

                var attacker = Parent as Mobile;

                if (attacker != null)
                {
                    /*
                     * Each weapon has a base and max range available to it, where the base
                     * range is modified by the player's strength to determine the actual range.
                     *
                     * Determining the maximum range of each weapon while in use:
                     * - Range = BaseRange + ((PlayerStrength - MinWeaponStrReq) / ((150 - MinWeaponStrReq) / 3))
                     * - The absolute maximum range is capped at 11 tiles.
                     *
                     * As per OSI tests: with 140 Strength you achieve max range for all throwing weapons.
                     */

                    int strReq = AosStrengthReq;
                    int divisor = (140 - strReq) / 3;

                    if (divisor <= 0)
                        divisor = 1;

                    int bonus = Math.Max(0, (attacker.Str - strReq) / divisor);
                    int range = (baseRange - 3) + bonus;

                    return Math.Min(11, Math.Max(baseRange - 3, range));
                }
                else
                {
                    return Math.Min(11, baseRange);
                }
            }
        }

        public override int EffectID
        {
            get
            {
                return ItemID;
            }
        }

        public override Type AmmoType
        {
            get
            {
                return null;
            }
        }

        public override Item Ammo
        {
            get
            {
                return null;
            }
        }

        public override int DefHitSound
        {
            get
            {
                return 0x5D3;
            }
        }

        public override int DefMissSound
        {
            get
            {
                return 0x5D4;
            }
        }

        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Throwing;
            }
        }

        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Throwing;
            }
        }

        public override SkillName AccuracySkill
        {
            get
            {
                return SkillName.Throwing;
            }
        }

        /*public override TimeSpan OnSwing(Mobile attacker, IDamageable damageable)
        {
            TimeSpan ts = base.OnSwing(attacker, damageable);

            // time it takes to throw it around including mystic arc
            if (ts < TimeSpan.FromMilliseconds(1000))
                ts = TimeSpan.FromMilliseconds(1000);

            return ts;
        }*/

        public override bool OnFired(Mobile attacker, IDamageable damageable)
        {
            m_Thrower = attacker;

            if (!attacker.InRange(damageable, 1))
            {
                attacker.MovingEffect(damageable, EffectID, 18, 1, false, false, Hue, 0);
            }

            return true;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            Point3D killSave = damageable.Location;
            Map map = attacker.Map;

            m_Thrower = attacker;
            m_Target = null;
            m_KillSave = killSave;

            if (!(WeaponAbility.GetCurrentAbility(attacker) is MysticArc))
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), () =>
                {
                    if (attacker != null && !attacker.Deleted && attacker.Map == map && map != null && map != Map.Internal)
                    {
                        Effects.SendMovingParticles(new Entity(Serial.Zero, killSave, map), attacker, EffectID, 18, 0, false, false, Hue, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    }

                    m_Target = null;
                    m_Thrower = null;
                });
            }

            base.OnHit(attacker, damageable, damageBonus);
        }

        public override void OnMiss(Mobile attacker, IDamageable damageable)
        {
            Mobile target = damageable as Mobile;
            Point3D missLoc = damageable.Location;
            Map map = attacker.Map;

            m_Thrower = attacker;
            m_Target = target;
            m_KillSave = missLoc;

            if (!(WeaponAbility.GetCurrentAbility(attacker) is MysticArc))
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), () =>
                {
                    if (attacker == null || attacker.Deleted || attacker.Map != map || map == null || map == Map.Internal)
                    {
                        m_Target = null;
                        m_Thrower = null;
                        return;
                    }

                    if (target != null && !target.Deleted && target.Map == map && attacker.InRange(target, 25))
                    {
                        target.MovingEffect(attacker, EffectID, 18, 1, false, false, Hue, 0);
                    }
                    else
                    {
                        Effects.SendMovingParticles(new Entity(Serial.Zero, missLoc, map), attacker, EffectID, 18, 0, false, false, Hue, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    }

                    m_Target = null;
                    m_Thrower = null;
                });
            }

            base.OnMiss(attacker, damageable);
        }

        public virtual void ThrowBack()
        {
            if (m_Thrower == null || m_Thrower.Deleted || m_Thrower.Map == null || m_Thrower.Map == Map.Internal)
            {
                m_Target = null;
                m_Thrower = null;
                return;
            }

            if (m_Target != null && !m_Target.Deleted && m_Target.Map == m_Thrower.Map && m_Thrower.InRange(m_Target, 25))
            {
                m_Target.MovingEffect(m_Thrower, EffectID, 18, 1, false, false, Hue, 0);
            }
            else if (m_Thrower.Map != null)
            {
                Effects.SendMovingParticles(new Entity(Serial.Zero, m_KillSave, m_Thrower.Map), m_Thrower, ItemID, 18, 0, false, false, Hue, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            }

            m_Target = null;
            m_Thrower = null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                InheritsItem = true;
        }

        #region Old Item Serialization Vars
        /* DO NOT USE! Only used in serialization of abyss reaver that originally derived from Item */
        public bool InheritsItem { get; protected set; }
        #endregion
    }
}
