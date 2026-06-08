using System;
using Server;

namespace Server.Items
{
    public class GargishBeastlyTunic : GargishPlateChest
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishBeastlyTunic()
        {
            Name = "Beastly Tunic";
            Hue = 1266;

            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 10;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 30;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;

            ArmorAttributes.MageArmor = 1;

            SkillBonuses.SetValues(0, BeastlyTunic.GetRandomSkill(), 20.0);
        }

        public GargishBeastlyTunic(Serial serial) : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
