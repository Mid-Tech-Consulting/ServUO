using System;

namespace Server.Items
{
    [Flipable(0xA40E, 0xA40E)]
    public class KaelvoksCincture : HalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public KaelvoksCincture()
        {
            ItemID = 0xA40E;
            Name = "Kaelvok's Cincture";
            Hue = 0x0AD0;
            Weight = 3.0;
            Layer = Layer.Waist;
            LootType = LootType.Blessed;

            Attributes.BonusStr = 5;
            Attributes.BonusHits = 3;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 10;
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SAAbsorptionAttributes.EaterDamage = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public KaelvoksCincture(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishKaelvoksCincture : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishKaelvoksCincture()
        {
            Name = "Gargish Kaelvok's Cincture";
            Hue = 0x0AD0;
            Weight = 2.0;
            LootType = LootType.Blessed;

            Attributes.BonusStr = 5;
            Attributes.BonusHits = 3;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 10;
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SAAbsorptionAttributes.EaterDamage = 10;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishKaelvoksCincture(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
