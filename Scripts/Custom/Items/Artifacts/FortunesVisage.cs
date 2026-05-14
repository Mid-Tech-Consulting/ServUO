using System;

using Server;

namespace Server.Items
{
    // Mage-armor wizard hat. Resists are Mage Armor + balanced 15% across
    // all five elements; +5 hits / +4 hp regen / +250 luck / 8 LMC.
    public class FortunesVisage : WizardsHat
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

            ClothingAttributes.MageArmor = 1;

            Resistances.Physical = Math.Max(0, 15 - BasePhysicalResistance);
            Resistances.Fire = Math.Max(0, 15 - BaseFireResistance);
            Resistances.Cold = Math.Max(0, 15 - BaseColdResistance);
            Resistances.Poison = Math.Max(0, 15 - BasePoisonResistance);
            Resistances.Energy = Math.Max(0, 15 - BaseEnergyResistance);

            StrRequirement = 30;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public FortunesVisage(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
