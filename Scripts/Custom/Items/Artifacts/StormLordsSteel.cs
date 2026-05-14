using System;

using Server;

namespace Server.Items
{
    // Exceptional Wakizashi: 80% Hit Lightning + 30% Hit Sparks, with the
    // Prized negative attribute (the "feels rare" tag). Pure physical damage.
    public class StormLordsSteel : Wakizashi
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public StormLordsSteel()
        {
            Name = "Storm Lord's Steel";
            Hue = 0x0ACC;
            Weight = 5.0;
            LootType = LootType.Regular;
            Quality = ItemQuality.Exceptional;

            WeaponAttributes.HitLightning = 80;
            ExtendedWeaponAttributes.HitSparks = 30;

            NegativeAttributes.Prized = 1;

            MinDamage = 10;
            MaxDamage = 14;
            Speed = 25; // 2.5s

            StrRequirement = 20;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public StormLordsSteel(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
