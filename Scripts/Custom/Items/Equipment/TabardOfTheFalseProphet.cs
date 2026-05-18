using System;

using Server;

namespace Server.Items
{
    public class TabardOfTheFalseProphet : BaseMiddleTorso
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TabardOfTheFalseProphet()
            : base(0xB1DE)
        {
            Name = "Tabard Of The False Prophet";
            Hue = 0x0AD4;
            Weight = 3.0;
            LootType = LootType.Regular;

            Attributes.BonusStr = 8;
            Attributes.BonusDex = 8;
            Attributes.BonusInt = 8;
            Attributes.BonusHits = 8;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 14;
            Attributes.WeaponDamage = 25;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;

            SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0);

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public TabardOfTheFalseProphet(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
