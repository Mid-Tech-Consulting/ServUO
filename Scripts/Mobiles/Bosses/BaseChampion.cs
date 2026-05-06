using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Services.Virtues;

namespace Server.Mobiles
{
    public enum ChampionTheme
    {
        None = 0,
        Undead,
        Demon,
        Reptile,
        Fey,
        Repond,
        Arachnid,
        Elemental,
    }

    public abstract class BaseChampion : BaseCreature
    {
        public BaseChampion(AIType aiType)
            : this(aiType, FightMode.Closest)
        {
        }

        public BaseChampion(AIType aiType, FightMode mode)
            : base(aiType, mode, 18, 1, 0.1, 0.2)
        {
        }

        public BaseChampion(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override bool AllureImmune { get { return true; } }
        public abstract ChampionSkullType SkullType { get; }
        public abstract Type[] UniqueList { get; }
        public abstract Type[] SharedList { get; }
        public abstract Type[] DecorativeList { get; }
        public abstract MonsterStatuetteType[] StatueTypes { get; }
        public virtual bool NoGoodies
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanGivePowerscrolls { get { return true; } }

        public static void GivePowerScrollTo(Mobile m, Item item, BaseChampion champ)
        {
            if (m == null)	//sanity
                return;

            if (!Core.SE || m.Alive)
                m.AddToBackpack(item);
            else
            {
                if (m.Corpse != null && !m.Corpse.Deleted)
                    m.Corpse.DropItem(item);
                else
                    m.AddToBackpack(item);
            }

            if (item is PowerScroll && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                for (int j = 0; j < pm.JusticeProtectors.Count; ++j)
                {
                    Mobile prot = pm.JusticeProtectors[j];

                    if (prot.Map != m.Map || prot.Murderer || prot.Criminal || !JusticeVirtue.CheckMapRegion(m, prot) || !prot.InRange(champ, 100))
                        continue;

                    int chance = 0;

                    switch( VirtueHelper.GetLevel(prot, VirtueName.Justice) )
                    {
                        case VirtueLevel.Seeker:
                            chance = 60;
                            break;
                        case VirtueLevel.Follower:
                            chance = 80;
                            break;
                        case VirtueLevel.Knight:
                            chance = 100;
                            break;
                    }

                    if (chance > Utility.Random(100))
                    {
						PowerScroll powerScroll = CreateRandomPowerScroll();

                        prot.SendLocalizedMessage(1049368); // You have been rewarded for your dedication to Justice!

                        if (!Core.SE || prot.Alive)
                            prot.AddToBackpack(powerScroll);
                        else
                        {
                            if (prot.Corpse != null && !prot.Corpse.Deleted)
                                prot.Corpse.DropItem(powerScroll);
                            else
                                prot.AddToBackpack(powerScroll);
                        }
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public virtual Item GetArtifact()
        {
            double random = Utility.RandomDouble();
            if (0.05 >= random)
                return this.CreateArtifact(this.UniqueList);
            else if (0.15 >= random)
                return this.CreateArtifact(this.SharedList);
            else if (0.30 >= random)
                return this.CreateArtifact(this.DecorativeList);
            return null;
        }

        public Item CreateArtifact(Type[] list)
        {
            if (list.Length == 0)
                return null;

            int random = Utility.Random(list.Length);
			
            Type type = list[random];

            Item artifact = Loot.Construct(type);

            if (artifact is MonsterStatuette && this.StatueTypes.Length > 0)
            {
                ((MonsterStatuette)artifact).Type = this.StatueTypes[Utility.Random(this.StatueTypes.Length)];
                ((MonsterStatuette)artifact).LootType = LootType.Regular;
            }

            return artifact;
        }

        public virtual void GivePowerScrolls()
        {
            // Siege ruleset — champion spawns on every facet that has them award power scrolls.
            var map = Map;
            if (map != Map.Felucca && map != Map.Ilshenar && map != Map.Tokuno && map != Map.Malas)
                return;

            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = GetLootingRights();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight && InRange(ds.m_Mobile, 100) && ds.m_Mobile.Map == this.Map)
                    toGive.Add(ds.m_Mobile);
            }

            if (toGive.Count == 0)
                return;

            for (int i = 0; i < toGive.Count; i++)
            {
                Mobile m = toGive[i];

                if (!(m is PlayerMobile))
                    continue;

                bool gainedPath = false;

                int pointsToGain = 800;

                if (VirtueHelper.Award(m, VirtueName.Valor, pointsToGain, ref gainedPath))
                {
                    if (gainedPath)
                        m.SendLocalizedMessage(1054032); // You have gained a path in Valor!
                    else
                        m.SendLocalizedMessage(1054030); // You have gained in Valor!
                    //No delay on Valor gains
                }
            }

            // Randomize - PowerScrolls
            for (int i = 0; i < toGive.Count; ++i)
            {
                int rand = Utility.Random(toGive.Count);
                Mobile hold = toGive[i];
                toGive[i] = toGive[rand];
                toGive[rand] = hold;
            }

            for (int i = 0; i < ChampionSystem.PowerScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                PowerScroll ps = CreateRandomPowerScroll();
                m.SendLocalizedMessage(1049524); // You have received a scroll of power!

                GivePowerScrollTo(m, ps, this);
            }

            if (Core.TOL)
            {
                // Randomize - Primers
                for (int i = 0; i < toGive.Count; ++i)
                {
                    int rand = Utility.Random(toGive.Count);
                    Mobile hold = toGive[i];
                    toGive[i] = toGive[rand];
                    toGive[rand] = hold;
                }

                for (int i = 0; i < ChampionSystem.PowerScrollAmount; ++i)
                {
                    Mobile m = toGive[i % toGive.Count];

                    SkillMasteryPrimer p = CreateRandomPrimer();
                    m.SendLocalizedMessage(1156209); // You have received a mastery primer!

                    GivePowerScrollTo(m, p, this);
                }
            }

            ColUtility.Free(toGive);
        }

        public virtual void OnChampPopped(ChampionSpawn spawn)
        {
        }

        public override bool OnBeforeDeath()
        {
            if (CanGivePowerscrolls && !NoKillAwards)
            {
                this.GivePowerScrolls();

                if (this.NoGoodies)
                    return base.OnBeforeDeath();

				GoldShower.DoForChamp(Location, Map);
            }

            return base.OnBeforeDeath();
        }

        #region Themed Artifact Drops
        // Each champion declares a Theme; the artifact pool is then themed to match.
        // Pool sizes are kept within 22-24 items so chase-pool odds are roughly
        // equal across themes (~4-5% per specific item per kill).

        private static readonly Type[] _UndeadPool = new Type[]
        {
            // Themed weapons (12)
            typeof(UndeadLeafblade), typeof(UndeadWarAxe), typeof(UndeadBroadsword),
            typeof(UndeadDoubleAxe), typeof(UndeadWarHammer), typeof(UndeadMagicalShortbow),
            typeof(UndeadCompositeBow), typeof(UndeadSoulGlaive), typeof(UndeadBoomerang),
            typeof(UndeadGargishTalwar), typeof(UndeadGargishKatana), typeof(UndeadLajatang),
            // Spellbook
            typeof(UndeadSpellbook),
            // Themed armor/clothing/jewelry
            typeof(GlovesOfTheArchlich), typeof(GargishKiltOfTheArchlich),
            typeof(MantleOfTheArchlich),
            typeof(MaskOfKhalAnkur),
            typeof(PendantOfKhalAnkur),
            typeof(ScabbardOfJuonar), typeof(GargishScabbardOfJuonar),
            // Themed talismans
            typeof(TalismanOfTheNecromancer),
            typeof(TalismanOfTheDeathKnight),
            typeof(TalismanOfTheStealthMage),
        };

        private static readonly Type[] _DemonPool = new Type[]
        {
            typeof(DemonLeafblade), typeof(DemonWarAxe), typeof(DemonBroadsword),
            typeof(DemonDoubleAxe), typeof(DemonWarHammer), typeof(DemonMagicalShortbow),
            typeof(DemonCompositeBow), typeof(DemonSoulGlaive), typeof(DemonBoomerang),
            typeof(DemonGargishTalwar), typeof(DemonGargishKatana), typeof(DemonLajatang),
            typeof(DemonSpellbook),
            typeof(BalronBoneArmor), typeof(GargishBalronBoneArmor),
            typeof(CorruptedPaladinVambraces), typeof(GargishCorruptedPaladinVambraces),
            typeof(ExporMalasFlamus), typeof(GargishExporMalasFlamus),
            typeof(ShadowMastersTalisman),
            typeof(TalismanOfTheCrusader),
            typeof(TalismanOfTheFencer),
            typeof(TalismanOfTheBrute),
        };

        private static readonly Type[] _ReptilePool = new Type[]
        {
            typeof(ReptileLeafblade), typeof(ReptileWarAxe), typeof(ReptileBroadsword),
            typeof(ReptileDoubleAxe), typeof(ReptileWarHammer), typeof(ReptileMagicalShortbow),
            typeof(ReptileCompositeBow), typeof(ReptileSoulGlaive), typeof(ReptileBoomerang),
            typeof(ReptileGargishTalwar), typeof(ReptileGargishKatana), typeof(ReptileLajatang),
            typeof(ReptilianDeathSpellbook),
            typeof(SerpentSkinQuiver), typeof(GargishSerpentSkinWingArmor),
            typeof(HexweaversVisage), typeof(GargishHexweaversVisage),
            typeof(UmbrascaleChampionsAegis), typeof(GargishUmbrascaleChampionsAegis),
            typeof(ShugenjasWand),
            typeof(TalismanOfTheMarksman),
            typeof(TalismanOfTheSkirmisher),
            typeof(TalismanOfTheBokutoMage),
        };

        private static readonly Type[] _FeyPool = new Type[]
        {
            typeof(FeyLeafblade), typeof(FeyWarAxe), typeof(FeyBroadsword),
            typeof(FeyDoubleAxe), typeof(FeyWarHammer), typeof(FeyMagicalShortbow),
            typeof(FeyCompositeBow), typeof(FeySoulGlaive), typeof(FeyBoomerang),
            typeof(FeyGargishTalwar), typeof(FeyGargishKatana), typeof(FeyLajatang),
            typeof(FeySpellbook),
            typeof(MushroomApron), typeof(GargishMushroomApron),
            typeof(RangersCloakOfAugmentation), typeof(WardensArmorOfAugmentation),
            typeof(KaelvoksCincture), typeof(GargishKaelvoksCincture),
            typeof(ShugenjasRaiment), typeof(GargishShugenjasRaiment),
            typeof(TalismanOfTheSpellweaver),
            typeof(TalismanOfTheMysticWarrior),
            // Bogling Hide Mukluks slot — race-picked at drop time, see GiveCustomArtifact.
            // Sentinel value below distinguishes the slot in the pool array.
            typeof(BoglingHideMukluks),
        };

        private static readonly Type[] _RepondPool = new Type[]
        {
            typeof(RepondLeafblade), typeof(RepondWarAxe), typeof(RepondBroadsword),
            typeof(RepondDoubleAxe), typeof(RepondWarHammer), typeof(RepondMagicalShortbow),
            typeof(RepondCompositeBow), typeof(RepondSoulGlaive), typeof(RepondBoomerang),
            typeof(RepondGargishTalwar), typeof(RepondGargishKatana), typeof(RepondLajatang),
            typeof(RepondSpellbook),
            typeof(GlovesOfTheHolyWarrior), typeof(GargishKiltOfTheHolyWarrior),
            typeof(SentinelsMempo), typeof(SentinelsNecklace),
            typeof(DeathwardensGreaves), typeof(GargishDeathwardensGreaves),
            typeof(TalismanOfTheWarrior),
            typeof(TalismanOfTheSamurai),
            typeof(TalismanOfTheTamerMage),
        };

        private static readonly Type[] _ArachnidPool = new Type[]
        {
            typeof(ArachnidLeafblade), typeof(ArachnidWarAxe), typeof(ArachnidBroadsword),
            typeof(ArachnidDoubleAxe), typeof(ArachnidWarHammer), typeof(ArachnidMagicalShortbow),
            typeof(ArachnidCompositeBow), typeof(ArachnidSoulGlaive), typeof(ArachnidBoomerang),
            typeof(ArachnidGargishTalwar), typeof(ArachnidGargishKatana), typeof(ArachnidLajatang),
            typeof(ArachnidDoomSpellbook),
            typeof(SolariasSecretPoisons), typeof(GargishSolariasSecretPoisons),
            typeof(LordMorphiusEpaulettes), typeof(GargishLordMorphiusEpaulettes),
            typeof(CarvedBoneRelicFromHolmes),
            typeof(ShadowbaneEpaulettes), typeof(GargishShadowbaneEpaulettes),
            typeof(TalismanOfTheNinja),
            typeof(TalismanOfTheMystic),
        };

        private static readonly Type[] _ElementalPool = new Type[]
        {
            typeof(ElementalLeafblade), typeof(ElementalWarAxe), typeof(ElementalBroadsword),
            typeof(ElementalDoubleAxe), typeof(ElementalWarHammer), typeof(ElementalMagicalShortbow),
            typeof(ElementalCompositeBow), typeof(ElementalSoulGlaive), typeof(ElementalBoomerang),
            typeof(ElementalGargishTalwar), typeof(ElementalGargishKatana), typeof(ElementalLajatang),
            typeof(ElementalBanSpellbook),
            typeof(AzaroksLegplates), typeof(GargishAzaroksLegplates),
            typeof(FeudalCloakOfElements), typeof(WingArmorOfElements),
            typeof(FeudalGhostwalkers), typeof(GargishFeudalGhostwalkers),
            typeof(GeneralLethesEpaulettes), typeof(GargishGeneralLethesEpaulettes),
            typeof(TalismanOfThePureMage),
            typeof(TalismanOfTheSpellsword),
        };

        private static readonly Dictionary<ChampionTheme, Type[]> _ThemedPools =
            new Dictionary<ChampionTheme, Type[]>
            {
                { ChampionTheme.Undead,    _UndeadPool    },
                { ChampionTheme.Demon,     _DemonPool     },
                { ChampionTheme.Reptile,   _ReptilePool   },
                { ChampionTheme.Fey,       _FeyPool       },
                { ChampionTheme.Repond,    _RepondPool    },
                { ChampionTheme.Arachnid,  _ArachnidPool  },
                { ChampionTheme.Elemental, _ElementalPool },
            };

        // Each champion subclass overrides this to indicate which themed pool it draws
        // from. Theme.None falls back to a random theme so unthemed champs still get
        // sensible loot until they're explicitly themed.
        public virtual ChampionTheme Theme { get { return ChampionTheme.None; } }

        // Sea champions (Corgul, Charybdis, Osiredon) have their own dedicated artifact
        // sets and are excluded from the standard land-champ themed drop pool — they
        // override this to false so they don't dilute what players are chasing.
        public virtual bool DropsThemedArtifacts { get { return true; } }

        public static void GiveCustomArtifact(Mobile m, ChampionTheme theme)
        {
            // Resolve None to a randomly-picked theme so future champions still get
            // a real pool until they override Theme explicitly.
            if (theme == ChampionTheme.None)
            {
                ChampionTheme[] themes = new ChampionTheme[]
                {
                    ChampionTheme.Undead, ChampionTheme.Demon, ChampionTheme.Reptile,
                    ChampionTheme.Fey, ChampionTheme.Repond, ChampionTheme.Arachnid,
                    ChampionTheme.Elemental
                };
                theme = themes[Utility.Random(themes.Length)];
            }

            Type[] pool;
            if (!_ThemedPools.TryGetValue(theme, out pool))
                return;

            Type type = pool[Utility.Random(pool.Length)];

            Item artifact;

            if (type == typeof(BoglingHideMukluks))
            {
                // Fey-pool sentinel — race-pick the gargoyle vs. human variant.
                artifact = m.Race == Race.Gargoyle
                    ? (Item)new GargishBoglingHideMukluks()
                    : new BoglingHideMukluks();
            }
            else
            {
                artifact = Loot.Construct(type);
            }

            if (artifact != null)
            {
                m.AddToBackpack(artifact);
                m.SendMessage(0x22, "You have received a champion artifact!");
            }
        }
        #endregion

        public override void OnDeath(Container c)
        {
            var map = Map;

            if (map == Map.Felucca || map == Map.Ilshenar || map == Map.Tokuno || map == Map.Malas)
            {
                List<DamageStore> rights = GetLootingRights();
                List<Mobile> toGive = new List<Mobile>();

                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight)
                        toGive.Add(ds.m_Mobile);
                }

                // Felucca-exclusive drops (skull, refinement) stay tied to canonical
                // power-scroll mechanics. The themed-artifact roll runs everywhere.
                if (map == Map.Felucca)
                {
                    if (SkullType != ChampionSkullType.None)
                    {
                        if (toGive.Count > 0)
                            toGive[Utility.Random(toGive.Count)].AddToBackpack(new ChampionSkull(SkullType));
                        else
                            c.DropItem(new ChampionSkull(SkullType));
                    }

                    if (Core.SA)
                        RefinementComponent.Roll(c, 3, 0.10);
                }

                // Themed artifact drops — 25% per eligible player on every champ facet.
                // Sea champions opt out via DropsThemedArtifacts (they have their own
                // dedicated High Seas artifact loot).
                if (DropsThemedArtifacts)
                {
                    foreach (Mobile m in toGive)
                    {
                        if (m is PlayerMobile && 0.25 > Utility.RandomDouble())
                        {
                            GiveCustomArtifact(m, Theme);
                        }
                    }
                }
            }

            base.OnDeath(c);
        }

        private static PowerScroll CreateRandomPowerScroll()
        {
            int level;
            double random = Utility.RandomDouble();

            if (0.15 >= random)
                level = 20;
            else if (0.5 >= random)
                level = 15;
            else
                level = 10;

            return PowerScroll.CreateRandomNoCraft(level, level);
        }

        private static SkillMasteryPrimer CreateRandomPrimer()
        {
            return SkillMasteryPrimer.GetRandom();
        }
    }
}