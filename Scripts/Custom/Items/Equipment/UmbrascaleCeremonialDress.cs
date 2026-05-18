using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleCeremonialDress : BaseOuterTorso
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public UmbrascaleCeremonialDress()
            : base(0xB2B9)
        {
            Name = "Umbrascale Ceremonial Dress";
            Hue = 0x0ADD;
            Weight = 3.0;
            LootType = LootType.Regular;

            SAAbsorptionAttributes.EaterFire = 15;
            Attributes.BonusInt = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public UmbrascaleCeremonialDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
