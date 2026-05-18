using System;

using Server;

namespace Server.Items
{
    public class VeilkeepersBranch : BaseStaff
    {
        public override bool IsArtifact { get { return true; } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ConcussionBlow; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Disarm; } }

        [Constructable]
        public VeilkeepersBranch()
            : base(0xA767)
        {
            Name = "Veilkeeper's Branch";
            Hue = 0x081C;
            Weight = 8.0;
            LootType = LootType.Regular;

            Attributes.SpellChanneling = 1;
            Attributes.BonusInt = 8;
            Attributes.BonusMana = 20;
            Attributes.Luck = 150;
            Attributes.SpellDamage = 16;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 0;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 15;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 25;

            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.MageWeapon = 10;

            SkillBonuses.SetValues(0, SkillName.Meditation, 20.0);
            SkillBonuses.SetValues(1, SkillName.Magery, 15.0);

            // Note: source data lists "Poison Resist 1%" -- ServUO weapons
            // don't expose elemental resist as a direct property. Skipping the
            // 1% trickle (negligible at that value).

            StrRequirement = 20;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public VeilkeepersBranch(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
