using System;

using Server;

namespace Server.Items
{
    // Gargoyle Divine Sanctifier (DiscMace base). Six "Hit X" procs make it
    // a sustain monster: full life/mana/stam leech, fatigue, lower defense,
    // and lightning. Plus DI+50.
    public class DivineSanctifierGargish : DiscMace
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public DivineSanctifierGargish()
        {
            Name = "Divine Sanctifier";
            Hue = 0x0ACC;
            Weight = 14.0;
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

            MinDamage = 11;
            MaxDamage = 15;
            Speed = 27; // 2.75s

            StrRequirement = 45;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public DivineSanctifierGargish(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
