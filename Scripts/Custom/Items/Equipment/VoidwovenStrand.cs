using System;

using Server;

namespace Server.Items
{
    public class VoidwovenStrand : JinBaori
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VoidwovenStrand()
        {
            Name = "Voidwoven Strand";
            Hue = 0x0AD7;
            Weight = 3.0;
            LootType = LootType.Blessed;

            Attributes.AttackChance = 5;
            Attributes.WeaponDamage = 10;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public VoidwovenStrand(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
