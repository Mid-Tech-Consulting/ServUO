using System;

using Server;

namespace Server.Items
{
    public class VoidskipperBoots : Boots
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VoidskipperBoots()
        {
            Name = "Voidskipper Boots";
            Hue = 0x0AD7;
            Weight = 3.0;
            LootType = LootType.Regular;

            Attributes.NightSight = 1;
            Attributes.Luck = 300;
            Attributes.LowerManaCost = 8;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.WeaponDamage = 15;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public VoidskipperBoots(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
