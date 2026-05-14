using System;

using Server;

namespace Server.Items
{
    // Bone-helm-style mage helm (graphic 0x1F0B = OrcHelm in ServUO art).
    // Mage Armor + balanced 15% across all five elements; +5 hits, +4 hp
    // regen, +250 luck, 8 LMC.
    public class FortunesVisage : OrcHelm
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FortunesVisage()
        {
            Name = "Fortune's Visage";
            Hue = 0x0780;
            Weight = 5.0;
            LootType = LootType.Regular;

            Attributes.BonusHits = 5;
            Attributes.RegenHits = 4;
            Attributes.Luck = 250;
            Attributes.LowerManaCost = 8;

            ArmorAttributes.MageArmor = 1;

            // Compensate for OrcHelm's base resists so the displayed total
            // lands at exactly 15 each.
            PhysicalBonus = Math.Max(0, 15 - BasePhysicalResistance);
            FireBonus = Math.Max(0, 15 - BaseFireResistance);
            ColdBonus = Math.Max(0, 15 - BaseColdResistance);
            PoisonBonus = Math.Max(0, 15 - BasePoisonResistance);
            EnergyBonus = Math.Max(0, 15 - BaseEnergyResistance);

            StrRequirement = 30;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public FortunesVisage(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
