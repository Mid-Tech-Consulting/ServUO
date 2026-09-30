using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public static class HalloweenSlayerHelper
    {
        public static string GetSlayerTitle(SlayerName slayer)
        {
            switch (slayer)
            {
                case SlayerName.Silver: return "Undead";
                case SlayerName.Exorcism: return "Demon";
                case SlayerName.ArachnidDoom: return "Arachnid";
                case SlayerName.ReptilianDeath: return "Reptile";
                case SlayerName.ElementalBan: return "Elemental";
                case SlayerName.Repond: return "Repond";
                default: return slayer.ToString();
            }
        }

        public static void ApplySlayerElements(BaseWeapon wep, SlayerName slayer)
        {
            wep.AosElementDamages.Physical = 0;
            wep.AosElementDamages.Fire = 0;
            wep.AosElementDamages.Cold = 0;
            wep.AosElementDamages.Poison = 0;
            wep.AosElementDamages.Energy = 0;

            wep.WeaponAttributes.HitFireArea = 0;
            wep.WeaponAttributes.HitColdArea = 0;
            wep.WeaponAttributes.HitEnergyArea = 0;
            wep.WeaponAttributes.HitPoisonArea = 0;
            wep.WeaponAttributes.HitPhysicalArea = 0;

            switch (slayer)
            {
                case SlayerName.ReptilianDeath:
                    wep.AosElementDamages.Cold = 100;
                    wep.WeaponAttributes.HitColdArea = 50;
                    break;
                case SlayerName.Exorcism:
                    wep.AosElementDamages.Cold = 100;
                    wep.WeaponAttributes.HitColdArea = 50;
                    break;
                case SlayerName.ElementalBan:
                    wep.AosElementDamages.Energy = 100;
                    wep.WeaponAttributes.HitEnergyArea = 50;
                    break;
                case SlayerName.Silver:
                case SlayerName.Repond:
                case SlayerName.ArachnidDoom:
                default:
                    wep.AosElementDamages.Fire = 100;
                    wep.WeaponAttributes.HitFireArea = 50;
                    break;
            }
        }

        public static string GetElementalSummary(SlayerName slayer)
        {
            switch (slayer)
            {
                case SlayerName.ReptilianDeath:
                case SlayerName.Exorcism:
                    return "100% Cold, 50% Hit Cold Area";
                case SlayerName.ElementalBan:
                    return "100% Energy, 50% Hit Energy Area";
                case SlayerName.Silver:
                case SlayerName.Repond:
                case SlayerName.ArachnidDoom:
                default:
                    return "100% Fire, 50% Hit Fire Area";
            }
        }
    }

    #region Halloween Composite Bow (Hellspire Spec)
    public class HalloweenCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public HalloweenCompositeBow() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HalloweenCompositeBow(SlayerName slayer)
        {
            Name = String.Format("Hellspire Composite Bow ({0} Slayer)", HalloweenSlayerHelper.GetSlayerTitle(slayer));
            Hue = 1358; // Vivid Pumpkin Hue

            Slayer = slayer;

            WeaponAttributes.HitLightning = 70;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLowerAttack = 60;
            WeaponAttributes.HitLowerDefend = 60;
            Velocity = 60;

            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 60;

            HalloweenSlayerHelper.ApplySlayerElements(this, slayer);
        }

        public HalloweenCompositeBow(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FF8C00>Hellspire Artifact Spec</BASEFONT>");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // Concrete slayer variants for direct [add commands
    public class HellspireUndeadCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireUndeadCompositeBow() : base(SlayerName.Silver) { }
        public HellspireUndeadCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireDemonCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireDemonCompositeBow() : base(SlayerName.Exorcism) { }
        public HellspireDemonCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireArachnidCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireArachnidCompositeBow() : base(SlayerName.ArachnidDoom) { }
        public HellspireArachnidCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireReptileCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireReptileCompositeBow() : base(SlayerName.ReptilianDeath) { }
        public HellspireReptileCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireElementalCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireElementalCompositeBow() : base(SlayerName.ElementalBan) { }
        public HellspireElementalCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireRepondCompositeBow : HalloweenCompositeBow
    {
        [Constructable] public HellspireRepondCompositeBow() : base(SlayerName.Repond) { }
        public HellspireRepondCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
    #endregion

    #region Halloween Soul Glaive (Hellspire Spec for Throwers)
    public class HalloweenSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public HalloweenSoulGlaive() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HalloweenSoulGlaive(SlayerName slayer)
        {
            Name = String.Format("Hellspire Soul Glaive ({0} Slayer)", HalloweenSlayerHelper.GetSlayerTitle(slayer));
            Hue = 1358; // Vivid Pumpkin Hue

            Slayer = slayer;

            WeaponAttributes.HitLightning = 70;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLowerAttack = 60;
            WeaponAttributes.HitLowerDefend = 60;
            Velocity = 60;

            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 60;

            HalloweenSlayerHelper.ApplySlayerElements(this, slayer);
        }

        public HalloweenSoulGlaive(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FF8C00>Hellspire Thrower Spec</BASEFONT>");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // Concrete slayer variants for direct [add commands
    public class HellspireUndeadSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireUndeadSoulGlaive() : base(SlayerName.Silver) { }
        public HellspireUndeadSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireDemonSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireDemonSoulGlaive() : base(SlayerName.Exorcism) { }
        public HellspireDemonSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireArachnidSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireArachnidSoulGlaive() : base(SlayerName.ArachnidDoom) { }
        public HellspireArachnidSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireReptileSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireReptileSoulGlaive() : base(SlayerName.ReptilianDeath) { }
        public HellspireReptileSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireElementalSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireElementalSoulGlaive() : base(SlayerName.ElementalBan) { }
        public HellspireElementalSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class HellspireRepondSoulGlaive : HalloweenSoulGlaive
    {
        [Constructable] public HellspireRepondSoulGlaive() : base(SlayerName.Repond) { }
        public HellspireRepondSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
    #endregion

    #region Halloween 64-Spell Grimoire
    public class HalloweenSpellbook : Spellbook
    {
        [Constructable]
        public HalloweenSpellbook() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HalloweenSpellbook(SlayerName slayer) : base(ulong.MaxValue) // Full 64 Magery Spells
        {
            Name = String.Format("Grimoire of the Crypt ({0} Slayer)", HalloweenSlayerHelper.GetSlayerTitle(slayer));
            Hue = 1161; // Candy Corn Orange
            LootType = LootType.Blessed;

            Slayer = slayer;

            Attributes.SpellDamage = 50;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 1;
            Attributes.RegenMana = 2;
        }

        public HalloweenSpellbook(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026</BASEFONT>");
            list.Add("<BASEFONT COLOR=#00FF00>Full 64 Magery Spells</BASEFONT>");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    // Concrete slayer variants for direct [add commands
    public class CryptUndeadSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptUndeadSpellbook() : base(SlayerName.Silver) { }
        public CryptUndeadSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CryptDemonSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptDemonSpellbook() : base(SlayerName.Exorcism) { }
        public CryptDemonSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CryptArachnidSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptArachnidSpellbook() : base(SlayerName.ArachnidDoom) { }
        public CryptArachnidSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CryptReptileSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptReptileSpellbook() : base(SlayerName.ReptilianDeath) { }
        public CryptReptileSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CryptElementalSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptElementalSpellbook() : base(SlayerName.ElementalBan) { }
        public CryptElementalSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class CryptRepondSpellbook : HalloweenSpellbook
    {
        [Constructable] public CryptRepondSpellbook() : base(SlayerName.Repond) { }
        public CryptRepondSpellbook(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
    #endregion
}
