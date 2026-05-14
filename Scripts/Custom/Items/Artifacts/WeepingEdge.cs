using System;

using Server;

namespace Server.Items
{
    // Exceptional Bokuto with 30% Splintering Weapon. Pure physical, fast.
    public class WeepingEdge : Bokuto
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public WeepingEdge()
        {
            Name = "Weeping Edge";
            Hue = 0x0B94;
            Weight = 7.0;
            LootType = LootType.Regular;
            Quality = ItemQuality.Exceptional;

            WeaponAttributes.SplinteringWeapon = 30;

            MinDamage = 10;
            MaxDamage = 12;
            Speed = 20; // 2.0s

            StrRequirement = 20;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public WeepingEdge(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
