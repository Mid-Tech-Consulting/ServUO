using System;
using Server;
using Server.Gumps;
using Server.TournamentSystem;

namespace Server.Mobiles
{
    public class ArenaKeeper : Healer
    {
        public PVPTournamentSystem System { get; set; }

        public ArenaKeeper(PVPTournamentSystem system)
        {
            System = system;
            Title = String.Format("the {0} Keeper", System.Name != null ? System.Name : "Arena");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!InRange(from, 3))
                base.OnDoubleClick(from);
            else if (System != null)
            {
                if (from.HasGump(typeof(TournamentStoneGump)))
                    from.CloseGump(typeof(TournamentStoneGump));

                if (from.HasGump(typeof(BaseTournamentGump)))
                    return;

                if(from is PlayerMobile)
                    BaseGump.SendGump(new TournamentStoneGump(System, from as PlayerMobile));

                SayTo(from, false, String.Format("Welcome to the {0}, {1}!", System.Name, from.Name));
            }
        }

        public override void OfferResurrection(Mobile m)
        {
            Direction = GetDirectionTo(m);

            m.PlaySound(0x1F2);
            m.FixedEffect(0x376A, 10, 16);
            m.Resurrect();

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), AfterResurrection, m);
        }

        public override bool CheckResurrect(Mobile m)
        {
            if (m.Criminal)
            {
                Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }

            return true;
        }

        public static void AfterResurrection(Mobile m)
        {
            if (m == null)
                return;

            if (m.Corpse != null)
            {
                Items.Corpse corpse = (Items.Corpse)m.Corpse;
                corpse.Location = m.Location;
                corpse.Map = m.Map;
                corpse.Open(m, true);
            }

            m.Hits = m.HitsMax;
            m.Mana = m.ManaMax;
            m.Stam = m.StamMax;

            ArenaHelper.DoArenaKeeperMessage(String.Format("Better luck next time, {0}!", m.Name), m);
        }

        public ArenaKeeper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
        	writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
    }
}