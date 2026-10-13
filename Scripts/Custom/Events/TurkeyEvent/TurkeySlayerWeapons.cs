using System;
using Server;
using Server.Items;

namespace Server.Custom.Events.TurkeyEvent
{
    public static class TurkeySlayerHelper
    {
        // 1192: Warm Autumn Yew Brown (medium rich wood brown with high contrast on paperdoll)
        public const int WeaponHue = 1192;

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

        public static void ApplyCommonProperties(BaseWeapon wep, SlayerName slayer)
        {
            wep.Slayer = slayer;
            wep.WeaponAttributes.HitLeechMana = 100;
            wep.WeaponAttributes.HitLeechHits = 100;
            wep.WeaponAttributes.HitLeechStam = 50;
            wep.WeaponAttributes.HitLowerDefend = 50;
            wep.WeaponAttributes.HitLightning = 70;

            wep.Attributes.Brittle = 1;
            wep.NegativeAttributes.Brittle = 1;

            wep.MaxHitPoints = 255;
            wep.HitPoints = 255;

            ApplySlayerElements(wep, slayer);
        }
    }

    #region Autumn Harvest Double Axe
    public class HarvestDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact => true;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public HarvestDoubleAxe() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HarvestDoubleAxe(SlayerName slayer)
        {
            Name = String.Format("Autumn Harvest Double Axe ({0} Slayer)", TurkeySlayerHelper.GetSlayerTitle(slayer));
            Hue = TurkeySlayerHelper.WeaponHue;
            TurkeySlayerHelper.ApplyCommonProperties(this, slayer);
        }

        public HarvestDoubleAxe(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Autumn Harvest Event 2026</BASEFONT>");
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
    #endregion

    #region Autumn Harvest Paladin Hammer
    public class HarvestPaladinHammer : WarHammer
    {
        public override bool IsArtifact => true;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public HarvestPaladinHammer() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HarvestPaladinHammer(SlayerName slayer)
        {
            Name = String.Format("Autumn Harvest Paladin Hammer ({0} Slayer)", TurkeySlayerHelper.GetSlayerTitle(slayer));
            Hue = TurkeySlayerHelper.WeaponHue;
            TurkeySlayerHelper.ApplyCommonProperties(this, slayer);
        }

        public HarvestPaladinHammer(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Autumn Harvest Event 2026</BASEFONT>");
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
    #endregion

    #region Autumn Harvest Lajatang
    public class HarvestLajatang : Lajatang
    {
        public override bool IsArtifact => true;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public HarvestLajatang() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HarvestLajatang(SlayerName slayer)
        {
            Name = String.Format("Autumn Harvest Lajatang ({0} Slayer)", TurkeySlayerHelper.GetSlayerTitle(slayer));
            Hue = TurkeySlayerHelper.WeaponHue;
            TurkeySlayerHelper.ApplyCommonProperties(this, slayer);
        }

        public HarvestLajatang(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Autumn Harvest Event 2026</BASEFONT>");
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
    #endregion

    #region Autumn Harvest Gargish Talwar
    public class HarvestGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact => true;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public HarvestGargishTalwar() : this(SlayerName.Silver)
        {
        }

        [Constructable]
        public HarvestGargishTalwar(SlayerName slayer)
        {
            Name = String.Format("Autumn Harvest Gargish Talwar ({0} Slayer)", TurkeySlayerHelper.GetSlayerTitle(slayer));
            Hue = TurkeySlayerHelper.WeaponHue;
            TurkeySlayerHelper.ApplyCommonProperties(this, slayer);
        }

        public HarvestGargishTalwar(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Autumn Harvest Event 2026</BASEFONT>");
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
    #endregion
}
