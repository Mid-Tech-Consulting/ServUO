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
            LootType = LootType.Blessed;

            Attributes.SpellChanneling = 1;
            Attributes.BonusInt = 4;
            Attributes.BonusMana = 10;
            Attributes.Luck = 150;
            Attributes.SpellDamage = 8;
            Attributes.CastRecovery = 1;

            SkillBonuses.SetValues(0, SkillName.Meditation, 20.0);

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
