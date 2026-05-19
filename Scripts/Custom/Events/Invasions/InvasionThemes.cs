using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Events
{
    // Central registry of every invasion theme. Add new themes by writing
    // another Build... method and including it in All.
    public static class InvasionThemes
    {
        public static readonly InvasionTheme Undead   = BuildUndead();
        public static readonly InvasionTheme Demon    = BuildDemon();
        public static readonly InvasionTheme Reptile  = BuildReptile();
        public static readonly InvasionTheme Arachnid = BuildArachnid();
        public static readonly InvasionTheme Fey      = BuildFey();
        public static readonly InvasionTheme Repond   = BuildRepond();

        public static IEnumerable<InvasionTheme> All
        {
            get
            {
                yield return Undead;
                yield return Demon;
                yield return Reptile;
                yield return Arachnid;
                yield return Fey;
                yield return Repond;
            }
        }

        // ---- Undead ----------------------------------------------------
        private static InvasionTheme BuildUndead()
        {
            InvasionTheme t = new InvasionTheme("Undead Invasion", typeof(BoneToken), "Bone Token");
            t.Description = "Hunt the unquiet dead. Bones turn in at the Undead Reward Stone.";
            t.StoneItemID = 0xEDC; t.StoneHue = 0x47E;
            t.SpawnerItemID = 0x1F1C; t.SpawnerHue = 0x47E;

            // Bounties roughly halved from the initial pass so the event
            // can run for months without players capping out the reward
            // catalog in a week. Trash stays at 1 (floor -- below this is
            // no drop at all).
            Add(t.Bounties, 1, typeof(Skeleton), typeof(Zombie), typeof(Bogle), typeof(Ghoul));
            Add(t.Bounties, 1, typeof(BoneKnight), typeof(BoneMagi), typeof(SkeletalKnight), typeof(SkeletalMage),
                                typeof(Mummy), typeof(Wraith), typeof(Spectre), typeof(Shade), typeof(Spellbinder));
            Add(t.Bounties, 2, typeof(Lich), typeof(RottingCorpse), typeof(DreamWraith), typeof(ShadowDweller), typeof(GargoyleShade));
            Add(t.Bounties, 7, typeof(LichLord), typeof(AncientLich), typeof(SkeletalDrake), typeof(SkeletalDragon), typeof(DarkGuardian), typeof(LadyOfTheSnow));

            t.Population[typeof(Skeleton)]       = 18;
            t.Population[typeof(Zombie)]         = 18;
            t.Population[typeof(Ghoul)]          = 12;
            t.Population[typeof(Bogle)]          = 11;
            t.Population[typeof(SkeletalKnight)] = 11;
            t.Population[typeof(SkeletalMage)]   = 11;
            t.Population[typeof(BoneKnight)]     = 10;
            t.Population[typeof(BoneMagi)]       = 10;
            t.Population[typeof(Mummy)]          = 10;
            t.Population[typeof(Wraith)]         = 10;
            t.Population[typeof(Lich)]           = 8;
            t.Population[typeof(RottingCorpse)]  = 7;
            t.Population[typeof(LichLord)]       = 5;
            t.Population[typeof(SkeletalDragon)] = 5;
            t.Population[typeof(LadyOfTheSnow)]  = 5;

            // Reward catalog -- 31 items kept from the original Undead event.
            t.Rewards.Add(new InvasionRewardEntry(typeof(AnkousSoulBinder),                "Ankou's Soul Binder",                200));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishAnkousSoulBinder),         "Ankou's Soul Binder (Gargish)",      200));
            t.Rewards.Add(new InvasionRewardEntry(typeof(ArtiosVineWrap),                  "Artio's Vine Wrap",                  200));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishArtiosVineWrap),           "Artio's Vine Wrap (Gargish)",        200));

            t.Rewards.Add(new InvasionRewardEntry(typeof(VoidwovenStrand),                 "Voidwoven Strand",                   300));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishVoidwovenStrand),          "Voidwoven Strand (Gargish)",         300));
            t.Rewards.Add(new InvasionRewardEntry(typeof(DupresSigil),                     "Dupre's Sigil",                      300));
            t.Rewards.Add(new InvasionRewardEntry(typeof(HexweaversIdol),                  "Hexweaver's Idol",                   300));
            t.Rewards.Add(new InvasionRewardEntry(typeof(RiftwardensPendant),              "Riftwarden's Pendant",               300));

            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleBattleRobe),            "Umbrascale Battle Robe",             400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleBattleDress),           "Umbrascale Battle Dress",            400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleGargishBattleRobe),     "Umbrascale Battle Robe (Gargish)",   400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleGargishBattleDress),    "Umbrascale Battle Dress (Gargish)",  400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleCeremonialRobe),        "Umbrascale Ceremonial Robe",         400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleCeremonialDress),       "Umbrascale Ceremonial Dress",        400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleGargishCeremonialRobe), "Umbrascale Ceremonial Robe (Gargish)",  400));
            t.Rewards.Add(new InvasionRewardEntry(typeof(UmbrascaleGargishCeremonialDress),"Umbrascale Ceremonial Dress (Gargish)", 400));

            t.Rewards.Add(new InvasionRewardEntry(typeof(VoidskipperBoots),                "Voidskipper Boots",                  500));
            t.Rewards.Add(new InvasionRewardEntry(typeof(RiftbreakerQuiver),               "Riftbreaker Quiver",                 500));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishRiftbreakerWing),          "Riftbreaker Wing (Gargish)",         500));

            t.Rewards.Add(new InvasionRewardEntry(typeof(FortunesVisage),                  "Fortune's Visage",                   600));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishFortunesVisage),           "Fortune's Visage (Gargish)",         600));
            t.Rewards.Add(new InvasionRewardEntry(typeof(TabardOfTheFalseProphet),         "Tabard of the False Prophet",        600));
            t.Rewards.Add(new InvasionRewardEntry(typeof(WeepingEdge),                     "Weeping Edge",                       800));
            t.Rewards.Add(new InvasionRewardEntry(typeof(SeaTempestsBulwark),              "Sea Tempest's Bulwark",             800));
            t.Rewards.Add(new InvasionRewardEntry(typeof(StormLordsSteel),                 "Storm Lord's Steel",                800));
            t.Rewards.Add(new InvasionRewardEntry(typeof(VeilkeepersBranch),               "Veilkeeper's Branch",               800));

            t.Rewards.Add(new InvasionRewardEntry(typeof(DivineSanctifier),                "Divine Sanctifier",                 1000));
            t.Rewards.Add(new InvasionRewardEntry(typeof(DivineSanctifierGargish),         "Divine Sanctifier (Gargish)",       1000));
            t.Rewards.Add(new InvasionRewardEntry(typeof(VoidTouchedCuirass),              "Void-Touched Cuirass",              1200));
            t.Rewards.Add(new InvasionRewardEntry(typeof(GargishVoidTouchedCuirass),       "Void-Touched Cuirass (Gargish)",    1200));

            return t;
        }

        // ---- Demon -----------------------------------------------------
        private static InvasionTheme BuildDemon()
        {
            InvasionTheme t = new InvasionTheme("Demon Invasion", typeof(BrimstoneToken), "Brimstone Bone");
            t.Description = "Drive the legions back. Brimstone turns in at the Demon Reward Stone.";
            t.StoneHue = 0x21; t.SpawnerHue = 0x21;

            Add(t.Bounties, 1,  typeof(Imp), typeof(HellHound), typeof(HellCat));
            Add(t.Bounties, 2,  typeof(Daemon), typeof(Devourer));
            Add(t.Bounties, 5,  typeof(Succubus), typeof(FireDaemon), typeof(FireSteed));
            Add(t.Bounties, 15, typeof(Balron), typeof(GreaterDragon));

            t.Population[typeof(Imp)]        = 24;
            t.Population[typeof(HellHound)]  = 18;
            t.Population[typeof(HellCat)]    = 12;
            t.Population[typeof(Daemon)]     = 9;
            t.Population[typeof(Devourer)]   = 6;
            t.Population[typeof(Succubus)]   = 6;
            t.Population[typeof(FireDaemon)] = 3;
            t.Population[typeof(Balron)]     = 3;

            // Rewards: fill in later. Stone shows "no rewards yet" until then.
            return t;
        }

        // ---- Reptile ---------------------------------------------------
        private static InvasionTheme BuildReptile()
        {
            InvasionTheme t = new InvasionTheme("Reptile Invasion", typeof(DragonscaleToken), "Dragonscale Bone");
            t.Description = "Slay the scaled host. Scales turn in at the Reptile Reward Stone.";
            t.StoneHue = 0x42; t.SpawnerHue = 0x42;

            Add(t.Bounties, 1,  typeof(Snake), typeof(GiantSerpent));
            Add(t.Bounties, 2,  typeof(IceSerpent), typeof(Drake), typeof(Wyvern));
            Add(t.Bounties, 5,  typeof(AncientWyrm));
            Add(t.Bounties, 15, typeof(Dragon));

            t.Population[typeof(Snake)]        = 18;
            t.Population[typeof(GiantSerpent)] = 18;
            t.Population[typeof(IceSerpent)]   = 12;
            t.Population[typeof(Drake)]        = 9;
            t.Population[typeof(Wyvern)]       = 9;
            t.Population[typeof(AncientWyrm)]  = 3;
            t.Population[typeof(Dragon)]       = 3;

            return t;
        }

        // ---- Arachnid --------------------------------------------------
        private static InvasionTheme BuildArachnid()
        {
            InvasionTheme t = new InvasionTheme("Arachnid Invasion", typeof(ChitinToken), "Chitin Bone");
            t.Description = "Burn out the nests. Chitin turns in at the Arachnid Reward Stone.";
            t.StoneHue = 0x497; t.SpawnerHue = 0x497;

            Add(t.Bounties, 1,  typeof(GiantSpider), typeof(Scorpion));
            Add(t.Bounties, 2,  typeof(FrostSpider), typeof(DreadSpider));
            Add(t.Bounties, 5,  typeof(TerathanWarrior), typeof(TerathanDrone), typeof(TerathanAvenger));
            Add(t.Bounties, 15, typeof(TerathanMatriarch), typeof(Mephitis), typeof(Navrey));

            t.Population[typeof(GiantSpider)]       = 18;
            t.Population[typeof(Scorpion)]          = 18;
            t.Population[typeof(FrostSpider)]       = 9;
            t.Population[typeof(DreadSpider)]       = 9;
            t.Population[typeof(TerathanWarrior)]   = 9;
            t.Population[typeof(TerathanDrone)]     = 9;
            t.Population[typeof(TerathanAvenger)]   = 6;
            t.Population[typeof(TerathanMatriarch)] = 3;

            return t;
        }

        // ---- Fey -------------------------------------------------------
        private static InvasionTheme BuildFey()
        {
            InvasionTheme t = new InvasionTheme("Fey Invasion", typeof(FeyToken), "Fey Bone");
            t.Description = "Stop the wild court's revel. Fey bones turn in at the Fey Reward Stone.";
            t.StoneHue = 0x1BF; t.SpawnerHue = 0x1BF;

            Add(t.Bounties, 1,  typeof(Wisp), typeof(Pixie), typeof(SAPixie));
            Add(t.Bounties, 2,  typeof(Centaur), typeof(Satyr), typeof(InsaneDryad));
            Add(t.Bounties, 5,  typeof(Treefellow), typeof(FeralTreefellow), typeof(TreefellowGuardian));
            Add(t.Bounties, 15, typeof(LordOaks), typeof(Silvani));

            t.Population[typeof(Pixie)]            = 18;
            t.Population[typeof(SAPixie)]          = 12;
            t.Population[typeof(Wisp)]             = 12;
            t.Population[typeof(Centaur)]          = 9;
            t.Population[typeof(Satyr)]            = 9;
            t.Population[typeof(InsaneDryad)]      = 6;
            t.Population[typeof(Treefellow)]       = 6;
            t.Population[typeof(FeralTreefellow)]  = 6;
            t.Population[typeof(TreefellowGuardian)] = 3;
            t.Population[typeof(LordOaks)]         = 3;

            return t;
        }

        // ---- Repond (humanoid) -----------------------------------------
        private static InvasionTheme BuildRepond()
        {
            InvasionTheme t = new InvasionTheme("Repond Invasion", typeof(RepondToken), "Repond Bone");
            t.Description = "Drive the savage tribes back. Repond bones turn in at the Repond Reward Stone.";
            t.StoneHue = 0x6BB; t.SpawnerHue = 0x6BB;

            Add(t.Bounties, 1,  typeof(Orc), typeof(Ratman), typeof(Savage));
            Add(t.Bounties, 2,  typeof(OrcishMage), typeof(OrcCaptain), typeof(RatmanArcher), typeof(RatmanMage),
                                 typeof(SavageRider), typeof(SavageShaman));
            Add(t.Bounties, 5,  typeof(OrcishLord), typeof(OrcBrute), typeof(Ettin), typeof(Ogre), typeof(Troll),
                                 typeof(Cyclops), typeof(MinotaurScout));
            Add(t.Bounties, 15, typeof(OgreLord), typeof(Titan), typeof(MinotaurCaptain), typeof(MinotaurGeneral));

            t.Population[typeof(Orc)]              = 18;
            t.Population[typeof(Ratman)]           = 18;
            t.Population[typeof(Savage)]           = 12;
            t.Population[typeof(OrcishMage)]       = 9;
            t.Population[typeof(OrcCaptain)]       = 9;
            t.Population[typeof(RatmanArcher)]     = 6;
            t.Population[typeof(RatmanMage)]       = 6;
            t.Population[typeof(SavageRider)]      = 6;
            t.Population[typeof(SavageShaman)]     = 6;
            t.Population[typeof(OrcishLord)]       = 6;
            t.Population[typeof(OrcBrute)]         = 6;
            t.Population[typeof(Ettin)]            = 6;
            t.Population[typeof(Ogre)]             = 6;
            t.Population[typeof(Troll)]            = 6;
            t.Population[typeof(Cyclops)]          = 3;
            t.Population[typeof(MinotaurScout)]    = 3;
            t.Population[typeof(OgreLord)]         = 3;
            t.Population[typeof(Titan)]            = 3;
            t.Population[typeof(MinotaurCaptain)]  = 3;

            return t;
        }

        private static void Add(Dictionary<Type, int> table, int amount, params Type[] types)
        {
            foreach (Type t in types)
                table[t] = amount;
        }
    }
}
