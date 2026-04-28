using System;

namespace Server.Items
{
    public class Tanto : BaseSword
    {
        [Constructable]
        public Tanto()
            : base(0xB4C7)
        {
            Weight = 8.0;
            Hue = 0x002E;
            Name = "Tanto";
        }

        public Tanto(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Feint; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.NerveStrike; } }

        public override int AosStrengthReq { get { return 20; } }
        public override int AosMinDamage { get { return 10; } }
        public override int AosMaxDamage { get { return 12; } }
        public override int AosSpeed { get { return 53; } }
        public override float MlSpeed { get { return 2.00f; } }

        public override int OldStrengthReq { get { return 20; } }
        public override int OldMinDamage { get { return 10; } }
        public override int OldMaxDamage { get { return 12; } }
        public override int OldSpeed { get { return 53; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 40; } }

        public override SkillName DefSkill { get { return SkillName.Fencing; } }
        public override WeaponType DefType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
