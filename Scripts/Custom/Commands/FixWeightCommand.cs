using Server;
using Server.Commands;
using Server.Targeting;

namespace Server.Custom.Commands
{
    // Admin helper: target a player and force a full UpdateTotals() recompute
    // on them. Useful when a player's encumbrance is stuck wrong because of a
    // bug elsewhere -- the recompute walks every equipped container and
    // rebuilds m_TotalGold / m_TotalItems / m_TotalWeight from scratch using
    // each item's TotalWeight + PileWeight, which resyncs any stale cached
    // deltas to the truth.
    //
    // Usage: [fixweight   -> target a mobile
    public static class FixWeightCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("fixweight", AccessLevel.GameMaster, OnCommand);
        }

        private static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage(0x40, "Target the character whose weight you want to recompute.");
            e.Mobile.Target = new FixWeightTarget();
        }

        private class FixWeightTarget : Target
        {
            public FixWeightTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is Mobile m))
                {
                    from.SendMessage(0x22, "Target a mobile.");
                    return;
                }

                int before = m.TotalWeight;

                m.UpdateTotals();

                int after = m.TotalWeight;

                from.SendMessage(0x40,
                    "{0}: weight recomputed {1} -> {2} (delta {3}).",
                    m.Name ?? m.GetType().Name, before, after, after - before);

                if (m != from && m.NetState != null)
                    m.SendMessage(0x40, "A staff member recomputed your carrying weight.");
            }
        }
    }
}
