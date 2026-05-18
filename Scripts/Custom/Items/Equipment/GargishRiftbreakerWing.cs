using System;

using Server;

namespace Server.Items
{
    // Gargoyle counterpart to the Riftbreaker Quiver. Wears in the wing
    // slot since gargoyles use thrown weapons instead of bows, so there's
    // no ammo capacity / lower-ammo-cost / quiver damage modifier -- those
    // stats are replaced by a +5 Throwing skill bump and the standard
    // wing-armor stat layout.
    public class GargishRiftbreakerWing : GargishLeatherWingArmor
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishRiftbreakerWing()
        {
            Name = "Riftbreaker Wing";
            Hue = 0x0AD7;
            LootType = LootType.Regular;

            Attributes.BonusDex = 8;
            Attributes.BonusStam = 15;
            Attributes.RegenStam = 2;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 25;
            Attributes.AttackChance = 10;
            Attributes.Luck = 150;

            SkillBonuses.SetValues(0, SkillName.Throwing, 5.0);
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishRiftbreakerWing(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
