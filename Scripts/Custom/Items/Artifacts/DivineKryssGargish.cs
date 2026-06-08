using System;

using Server;

namespace Server.Items
{
    public class DivineKryssGargish : GargishKryss
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public DivineKryssGargish()
        {
            Name = "Divine Kryss";
            Hue = 0x0ACC;
            Weight = 2.0;
            LootType = LootType.Regular;

            WeaponAttributes.HitLeechMana = 100;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitLeechStam = 50;
            WeaponAttributes.HitFatigue = 70;
            WeaponAttributes.HitLowerDefend = 50;
            WeaponAttributes.HitLightning = 70;

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 30;
            Attributes.LowerManaCost = 8;

            MinDamage = 10;
            MaxDamage = 12;
            Speed = 2.0f;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public DivineKryssGargish(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
