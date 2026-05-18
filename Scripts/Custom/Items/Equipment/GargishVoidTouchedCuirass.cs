using Server;

namespace Server.Items
{
    // Gargoyle equivalent of VoidTouchedCuirass -- same stats, gargoyle
    // plate-chest slot, race-locked.
    public class GargishVoidTouchedCuirass : GargishPlateChest
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishVoidTouchedCuirass()
        {
            Name = "Void-Touched Cuirass";
            Hue = 0x0AD7;
            Weight = 10.0;
            LootType = LootType.Regular;

            SkillBonuses.SetValues(0, BalronBoneArmor.GetRandomGargoyleCombatSkill(), 20.0);
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 15;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;

            ArmorAttributes.MageArmor = 1;

            StrRequirement = 60;
            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

        public GargishVoidTouchedCuirass(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
