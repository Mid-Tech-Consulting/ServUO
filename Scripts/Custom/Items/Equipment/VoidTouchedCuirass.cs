using System;

using Server;

namespace Server.Items
{
    public class VoidTouchedCuirass : PlateChest
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VoidTouchedCuirass()
        {
            Name = "Void-Touched Cuirass";
            Hue = 0x0AD7;
            Weight = 10.0;
            LootType = LootType.Blessed;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;

            // Resist bonuses: compensate for plate base resists so the displayed
            // total lands at exactly 15 each.
            PhysicalBonus = Math.Max(0, 15 - BasePhysicalResistance);
            FireBonus = Math.Max(0, 15 - BaseFireResistance);
            ColdBonus = Math.Max(0, 15 - BaseColdResistance);
            PoisonBonus = Math.Max(0, 15 - BasePoisonResistance);
            EnergyBonus = Math.Max(0, 15 - BaseEnergyResistance);

            StrRequirement = 95;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public VoidTouchedCuirass(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
