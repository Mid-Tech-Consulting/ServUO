using System;

namespace Server.Items
{
    public class BoglingHideMukluks : Boots
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BoglingHideMukluks()
            : base(0x03FA)
        {
            Name = "Bogling Hide Mukluks";
            Weight = 1.0;
            LootType = LootType.Blessed;

            StrRequirement = 10;
            MaxHitPoints = 255;
            HitPoints = 255;

            Attributes.BonusStr = 3;
            Attributes.BonusDex = 3;
            Attributes.BonusInt = 3;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
        }

        public BoglingHideMukluks(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
