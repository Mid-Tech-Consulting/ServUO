using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Events
{
    // Generic loot hook for every Invasion theme. Subscribes once to
    // EventSink.OnKilledBy at server start, then dispatches each kill
    // through the active themes -- the first theme whose bounty table
    // claims the mob's type pays out its themed token. So Skeleton dies
    // -> Bone Token, Daemon dies -> Brimstone Bone, etc.
    public static class InvasionLoot
    {
        // Type -> (theme, amount). Built once at server start so each kill
        // is a single dictionary lookup.
        private static Dictionary<Type, BountyHit> _Lookup;

        // Per-theme refcount of currently-active spawners. A theme drops
        // tokens iff this count is > 0 -- so killing a wild Dragon in some
        // random forest doesn't hand out Dragonscale Bones unless a Reptile
        // Invasion is actually running (i.e. a spawner is toggled Active).
        private static readonly Dictionary<InvasionTheme, int> _ActiveCounts =
            new Dictionary<InvasionTheme, int>();

        private struct BountyHit
        {
            public InvasionTheme Theme;
            public int Amount;
        }

        public static void Initialize()
        {
            _Lookup = new Dictionary<Type, BountyHit>();

            foreach (InvasionTheme theme in InvasionThemes.All)
            {
                foreach (KeyValuePair<Type, int> kv in theme.Bounties)
                {
                    // First theme to register a type wins. Avoids ambiguity
                    // when a mob like SkeletalDragon could plausibly belong
                    // to multiple themes.
                    if (!_Lookup.ContainsKey(kv.Key))
                        _Lookup[kv.Key] = new BountyHit { Theme = theme, Amount = kv.Value };
                }
            }

            EventSink.OnKilledBy += OnKilledBy;
        }

        // Spawners call this when toggling Active or being created/destroyed.
        public static void NotifyActive(InvasionTheme theme, bool active)
        {
            if (theme == null)
                return;

            int count;
            _ActiveCounts.TryGetValue(theme, out count);

            count += active ? 1 : -1;
            if (count < 0)
                count = 0;

            _ActiveCounts[theme] = count;
        }

        public static bool IsThemeActive(InvasionTheme theme)
        {
            int count;
            return theme != null && _ActiveCounts.TryGetValue(theme, out count) && count > 0;
        }

        private static void OnKilledBy(OnKilledByEventArgs e)
        {
            if (e == null || e.Killed == null || e.KilledBy == null)
                return;

            BaseCreature dead = e.Killed as BaseCreature;
            if (dead == null)
                return;

            PlayerMobile killer = e.KilledBy as PlayerMobile;
            if (killer == null || !killer.Alive)
                return;

            BountyHit hit;
            if (!_Lookup.TryGetValue(dead.GetType(), out hit) || hit.Amount <= 0)
                return;

            // No active spawner for this theme = event isn't running = no
            // tokens. Players need a GM-placed and Activated spawner before
            // their kills count.
            if (!IsThemeActive(hit.Theme))
                return;

            Item award = Activator.CreateInstance(hit.Theme.TokenType, new object[] { hit.Amount }) as Item;
            if (award == null)
                return;

            if (!killer.PlaceInBackpack(award))
            {
                award.MoveToWorld(killer.Location, killer.Map);
                killer.SendMessage(hit.Theme.StoneHue == 0 ? 0x47E : hit.Theme.StoneHue,
                    String.Format("Your pack is full -- a {0} fell at your feet.", hit.Theme.TokenName));
            }
            else
            {
                killer.SendMessage(hit.Theme.StoneHue == 0 ? 0x47E : hit.Theme.StoneHue,
                    String.Format("You receive {0} {1}{2}.", hit.Amount, hit.Theme.TokenName, hit.Amount == 1 ? "" : "s"));
            }
        }
    }
}
