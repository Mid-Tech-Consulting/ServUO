using System;

using Server;

namespace Server.Items
{
    // Mage off-hand shield disguised as a lit torch. Subclasses MetalShield
    // so it benefits from the Parry skill and standard shield mechanics
    // (resists, hits, AR scaling), but uses the lit-torch graphic for the
    // "branch" theme. SpellChanneling means no spell-cast interruption when
    // equipped, so casters can hold it permanently.
    public class VeilkeepersBranch : MetalShield
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VeilkeepersBranch()
        {
            ItemID = 0xA12;        // lit torch graphic
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
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 15;
            Attributes.DefendChance = 15;

            SkillBonuses.SetValues(0, SkillName.Meditation, 20.0);
            SkillBonuses.SetValues(1, SkillName.Magery, 15.0);

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        // MetalShield baseline is 0 across the board -- give a modest mage
        // resist spread so it pulls weight as an off-hand piece.
        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance     { get { return 8; } }
        public override int BaseColdResistance     { get { return 5; } }
        public override int BasePoisonResistance   { get { return 5; } }
        public override int BaseEnergyResistance   { get { return 8; } }

        public VeilkeepersBranch(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
