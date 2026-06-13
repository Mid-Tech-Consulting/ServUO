using System;
using Server;

namespace Server.Items
{
    public class GargishTwiceMittens : GargishLeatherKilt
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int BasePhysicalResistance { get { return 14; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 18; } }
        public override int BasePoisonResistance { get { return 13; } }
        public override int BaseEnergyResistance { get { return 14; } }

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

        [Constructable]
        public GargishTwiceMittens()
        {
            Name = "Twice Mittens";
            Hue = 18;
            LootType = LootType.Regular;

            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusMana = 5;
            Attributes.BonusStam = 5;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 20;
            Attributes.RegenMana = 2;
            Attributes.WeaponDamage = 20;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
        }

        public GargishTwiceMittens(Serial serial) : base(serial)
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
