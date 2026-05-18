using System;

using Server;

namespace Server.Items
{
    public class SeaTempestsBulwark : BaseShield
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SeaTempestsBulwark()
            : base(0xA649)
        {
            Name = "Sea Tempest's Bulwark";
            Hue = 0x047E;
            Weight = 8.0;
            LootType = LootType.Regular;

            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenHits = 2;
            Attributes.Luck = 200;
            Attributes.DefendChance = 15;
            Attributes.AttackChance = 5;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 15;
            Attributes.SpellChanneling = 1;

            SkillBonuses.SetValues(0, SkillName.Magery, 10.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 10.0);

            ArmorAttributes.SoulCharge = 30;
            ArmorAttributes.ReactiveParalyze = 1;

            FireBonus = 5;
            ColdBonus = 10;
            EnergyBonus = 5;

            StrRequirement = 20;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public SeaTempestsBulwark(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
