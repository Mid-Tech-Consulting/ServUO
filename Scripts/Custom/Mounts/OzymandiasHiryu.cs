using Server.Items;
using Server.Multis;
using System;

namespace Server.Mobiles
{
    [CorpseName("an Ozymandias' hiryu corpse")]
    public class OzymandiasHiryu : Hiryu
    {
        private static readonly TrainingDefinition _TrainingDef = new TrainingDefinition(
            typeof(OzymandiasHiryu),
            Class.ClawedTailedMagicalAndTokuno,
            MagicalAbility.Hiryu,
            PetTrainingHelper.SpecialAbilityNone,
            PetTrainingHelper.WepAbility7,
            PetTrainingHelper.AreaEffectArea1,
            1,
            5);

        public override TrainingDefinition TrainingDefinition { get { return _TrainingDef; } }

        [Constructable]
        public OzymandiasHiryu()
            : base()
        {
            Name = "Ozymandias' Hiryu";
            Body = 0x00F3;
            Hue = 0x09C4;

            // 1-slot starter stats. Tamers train it up via the pet-training system.
            SetStr(100, 130);
            SetDex(70, 90);
            SetInt(60, 80);

            SetHits(70, 100);

            SetDamage(5, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Anatomy, 45.0, 55.0);
            SetSkill(SkillName.MagicResist, 55.0, 65.0);
            SetSkill(SkillName.Tactics, 50.0, 60.0);
            SetSkill(SkillName.Wrestling, 50.0, 60.0);

            Fame = 2500;
            Karma = -2500;

            ControlSlots = 1;
            MinTameSkill = 0.0;
        }

        public OzymandiasHiryu(Serial serial)
            : base(serial)
        {
        }

        // Anyone can ride the Hiryu while it's untrained (still 1 slot).
        // Once a tamer trains it above 1 slot, normal owner-only rules apply.
        public override void OnDoubleClick(Mobile from)
        {
            if (IsDeadPet)
                return;

            if (from.IsBodyMod && !from.Body.IsHuman)
            {
                if (Core.AOS)
                    PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1062061, from.NetState); // You cannot ride a mount in your current form.
                else
                    from.SendLocalizedMessage(1061628); // You can't do that while polymorphed.

                return;
            }

            if (!CheckMountAllowed(from, this, true, false))
                return;

            if (from.Mount is BaseBoat)
                return;

            if (from.Mounted)
            {
                from.SendLocalizedMessage(1005583); // Please dismount first.
                return;
            }

            if (from.Race == Race.Gargoyle && from.IsPlayer())
            {
                from.SendLocalizedMessage(1112281);
                OnDisallowedRider(from);
                return;
            }

            if (from.Female ? !AllowFemaleRider : !AllowMaleRider)
            {
                OnDisallowedRider(from);
                return;
            }

            if (!DesignContext.Check(from))
                return;

            if (from.HasTrade)
            {
                from.SendLocalizedMessage(1042317); // You may not ride at this time
                return;
            }

            if (from.InRange(this, 1))
            {
                bool trained = ControlSlots > ControlSlotsMin;
                bool canAccess = !trained
                    || from.AccessLevel >= AccessLevel.GameMaster
                    || (Controlled && ControlMaster == from);

                if (!canAccess)
                {
                    // This isn't your mount; it refuses to let you ride.
                    PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 501264, from.NetState);
                }
                else if (Poisoned)
                {
                    PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1049692, from.NetState); // This mount is too ill to ride.
                }
                else
                {
                    Rider = from;
                }
            }
            else
            {
                from.SendLocalizedMessage(500206); // That is too far away to ride.
            }
        }

        // Mirror ParoxysmusSwampDragon: 20% PvM damage absorbed for the player rider, unbreakable.
        public override void OnRiderDamaged(Mobile from, ref int amount, bool willKill)
        {
            base.OnRiderDamaged(from, ref amount, willKill);

            if (Rider == null)
                return;

            if ((from == null || !from.Player) && Rider.Player && Rider.Mount == this)
            {
                int absorbed = AOS.Scale(amount, 20);
                amount -= absorbed;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
