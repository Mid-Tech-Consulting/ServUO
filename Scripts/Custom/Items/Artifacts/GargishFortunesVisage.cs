using System;

using Server;

namespace Server.Items
{
    // Gargoyle counterpart to Fortune's Visage. Same stat block, but lives
    // on the gargoyle eyewear slot (GargishGlasses / Layer.Earrings) instead
    // of an OrcHelm. Resist bonuses compensate for the GargishGlasses base
    // resists so the displayed total still lands at 15 each.
    public class GargishFortunesVisage : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishFortunesVisage()
        {
            Name = "Fortune's Visage";
            Hue = 0x0780;
            Weight = 2.0;
            LootType = LootType.Regular;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 8;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.RegenHits = 4;
            Attributes.Luck = 250;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 15;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;

            SkillBonuses.SetValues(0, BalronBoneArmor.GetRandomGargoyleCombatSkill(), 15.0);

            ArmorAttributes.MageArmor = 1;

            PhysicalBonus = Math.Max(0, 15 - BasePhysicalResistance);
            FireBonus = Math.Max(0, 15 - BaseFireResistance);
            ColdBonus = Math.Max(0, 15 - BaseColdResistance);
            PoisonBonus = Math.Max(0, 15 - BasePoisonResistance);
            EnergyBonus = Math.Max(0, 15 - BaseEnergyResistance);

            StrRequirement = 30;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishFortunesVisage(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
