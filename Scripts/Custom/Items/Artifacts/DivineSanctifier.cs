using System;

using Server;

namespace Server.Items
{
    // Human (HammerPick base) Divine Sanctifier counterpart -- same hit
    // procs as the gargoyle version, slightly slower and harder hitting.
    public class DivineSanctifier : HammerPick
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DivineSanctifier()
        {
            Name = "Divine Sanctifier";
            Hue = 0x0ACC;
            Weight = 9.0;
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

            MinDamage = 13;
            MaxDamage = 17;
            Speed = 32; // 3.25s rounded; ServUO uses tenths-of-second convention

            StrRequirement = 45;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public DivineSanctifier(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
