using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleGargishCeremonialDress : BaseClothing
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public UmbrascaleGargishCeremonialDress()
            : base(0xB2BA, Layer.OuterTorso)
        {
            Name = "Umbrascale Gargish Ceremonial Dress";
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

        public UmbrascaleGargishCeremonialDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
