using System;

using Server;

namespace Server.Items
{
    public class DupresSigil : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DupresSigil()
            : base(0x2F5B)
        {
            Name = "Dupre's Sigil";
            Hue = 0x0ADF;
            Weight = 1.0;
            LootType = LootType.Blessed;

            Removal = TalismanRemoval.Ward;

            Attributes.ReflectPhysical = 20;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponDamage = 20;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public DupresSigil(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
