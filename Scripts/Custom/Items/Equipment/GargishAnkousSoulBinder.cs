using System;

using Server;

namespace Server.Items
{
    public class GargishAnkousSoulBinder : GargishSash
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishAnkousSoulBinder()
        {
            Name = "Ankou's Soul Binder";
            Hue = 0x0AD7;
            Weight = 1.0;
            LootType = LootType.Regular;

            // Mirrors the human AnkousSoulBinder. Trades Artio's +1 RegenMana
            // / NightSight / +5 Magery for +2 Mana / +2 LMC / +4 SDI / +5
            // Magic Resist.
            Attributes.BonusInt = 5;
            Attributes.BonusMana = 12;
            Attributes.RegenMana = 2;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 2;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 15;
            Attributes.SpellDamage = 12;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 5.0);

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishAnkousSoulBinder(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
