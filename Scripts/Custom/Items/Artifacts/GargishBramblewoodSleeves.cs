using System;
using Server;

namespace Server.Items
{
    public class GargishBramblewoodSleeves : GargishLeatherArms
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int BasePhysicalResistance { get { return 12; } }
        public override int BaseFireResistance { get { return 12; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 12; } }
        public override int BaseEnergyResistance { get { return 8; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public GargishBramblewoodSleeves()
        {
            Name = "Bramblewood Sleeves";
            Hue = 68;
            LootType = LootType.Regular;

            Attributes.AttackChance = 5;
            Attributes.BonusDex = 10;
            Attributes.BonusStam = 10;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 20;
            Attributes.Luck = 200;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 5;
            Attributes.WeaponSpeed = 5;

            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
        }

        public GargishBramblewoodSleeves(Serial serial) : base(serial)
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
