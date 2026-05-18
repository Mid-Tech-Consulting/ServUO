using System;

using Server;

namespace Server.Items
{
    public class RiftbreakerQuiver : BaseQuiver
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public RiftbreakerQuiver()
            : base(0x2B02)
        {
            Name = "Riftbreaker Quiver";
            Hue = 0x0AD7;
            Weight = 8.0;
            LootType = LootType.Regular;

            Capacity = 1000;
            // Quiver DamageIncrease is applied AFTER the standard 100% DI
            // cap as a post-formula multiplier (AOS.cs), so it stacks on top
            // of capped gear DI. Held at 10 to match OSI artifact tier
            // (Quiver of Infinity / Rage / Serpent Skin) -- higher values
            // give archers a flat damage advantage no other weapon class
            // can match.
            DamageIncrease = 10;
            LowerAmmoCost = 30;
            WeightReduction = 30;

            Attributes.BonusDex = 8;
            Attributes.BonusStam = 15;
            Attributes.RegenStam = 2;
            Attributes.WeaponSpeed = 10;
            Attributes.AttackChance = 10;
            Attributes.Luck = 150;

            SkillBonuses.SetValues(0, SkillName.Archery, 5.0);
        }

        public RiftbreakerQuiver(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
