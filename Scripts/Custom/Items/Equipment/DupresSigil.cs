using System;

using Server;

namespace Server.Items
{
    public class DupresSigil : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        // Keep the custom name visible even though Removal is set --
        // BaseTalisman normally replaces it with "Talisman of Ward Removal"
        // when Removal != None.
        public override bool ForceShowName { get { return true; } }

        [Constructable]
        public DupresSigil()
            : base(0x2F5B)
        {
            Name = "Dupre's Sigil";
            Hue = 0x0ADF;
            Weight = 1.0;
            LootType = LootType.Regular;

            // Ward Removal -- double-click + target a player to strip
            // beneficial wards (Gift of Renewal, Confidence, etc).
            // MaxChargeTime gives the standard 20-minute recharge cycle
            // and makes "Fully Charged" appear in the tooltip.
            Removal = TalismanRemoval.Ward;
            MaxChargeTime = 1200;

            Attributes.BonusStr = 8;
            Attributes.BonusHits = 8;
            Attributes.RegenHits = 3;
            Attributes.ReflectPhysical = 25;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 5.0);

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        // BaseTalisman skips the "Talisman of ~name~" line when ForceShowName
        // is true (that branch normally replaces the custom name). Re-add it
        // here so the tooltip shows "Talisman of Ward Removal" under the
        // custom name.
        public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add(1072389, "#" + (1072000 + (int)Removal)); // Talisman of ~1_name~
        }

        public DupresSigil(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
