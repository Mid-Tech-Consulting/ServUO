using System;
using Server;

namespace Server.Items
{
    public class BeastlyTunic : PlateChest
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return null; } }

        [Constructable]
        public BeastlyTunic()
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

            SkillBonuses.SetValues(0, GetRandomSkill(), 20.0);
        }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[]
            {
                SkillName.Mysticism, SkillName.AnimalLore, SkillName.AnimalTaming,
                SkillName.Spellweaving, SkillName.Necromancy
            };
            return skills[Utility.Random(skills.Length)];
        }

        public BeastlyTunic(Serial serial) : base(serial)
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
            writer.Write((int)3); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
