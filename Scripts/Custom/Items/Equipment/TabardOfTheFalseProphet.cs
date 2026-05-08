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
            LootType = LootType.Blessed;

            Attributes.BonusStr = 4;
            Attributes.BonusDex = 4;
            Attributes.BonusInt = 4;
            Attributes.SpellDamage = 6;
            Attributes.WeaponDamage = 15;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public TabardOfTheFalseProphet(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
