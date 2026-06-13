using System;
using Server;

namespace Server.Items
{
    public class GargishNeckGuard : GargishNecklace
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

        public override int InitMinHits { get { return 175; } }
        public override int InitMaxHits { get { return 175; } }

        [Constructable]
        public GargishNeckGuard()
        {
            Name = "Neck Guard";
            Hue = 43;
            LootType = LootType.Regular;

            Attributes.AttackChance = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 10;
            Attributes.BonusStam = 10;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 20;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 5;
            Attributes.WeaponDamage = 5;

            SkillBonuses.SetValues(0, SkillName.Focus, 10.0);
        }

        public GargishNeckGuard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
