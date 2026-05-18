using System;

using Server;

namespace Server.Items
{
    // Mage sash: +5 mana, +2 mana regen, +1 FCR. Light str req (10).
    public class ArtiosVineWrap : BodySash
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ArtiosVineWrap()
        {
            Name = "Artio's Vine Wrap";
            Hue = 0x0497;
            Weight = 1.0;
            LootType = LootType.Regular;

            Attributes.BonusInt = 5;
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 15;
            Attributes.SpellDamage = 8;
            Attributes.NightSight = 1;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public ArtiosVineWrap(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
