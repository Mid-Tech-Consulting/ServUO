using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.TournamentSystem
{
	public class FlagHolder : Item
	{
        public static int FreezeAfterRez = 30; // in seconds

		private ArenaTeam m_Owner;
		private CTFFlag m_HomeFlag;
		private CTFFlag m_EnemyFlag;
		private DateTime m_FlagPlaced;
		private PVPTournamentSystem m_System;
		
		[CommandProperty(AccessLevel.GameMaster)]
        public ArenaTeam Owner { get { return m_Owner; } set { m_Owner = value; InvalidateProperties(); } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public CTFFlag HomeFlag { get { return m_HomeFlag; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public CTFFlag EnemyFlag { get { return m_EnemyFlag; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime FlagPlaced { get { return m_FlagPlaced; } set { m_FlagPlaced = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public PVPTournamentSystem System { get { return m_System; } set { m_System = value; } }
		
		public override string DefaultName { get { return "Flag Holder"; } }
        public override bool ForceShowProperties { get { return true; } }

		public FlagHolder(PVPTournamentSystem system) : base(0x0077)
		{
			m_System = system;
			Movable = false;
		}
		
		public void Reset()
		{
			m_Owner = null;
			m_FlagPlaced = DateTime.MinValue;
			m_HomeFlag = null;
			m_EnemyFlag = null;
		}

        public bool IsHomeFlag(CTFFlag flag)
        {
            return flag.Owner == m_Owner;
        }
		
		public void AddFlag(CTFFlag flag)
		{
			if(flag.Owner != m_Owner)
			{
				m_EnemyFlag = flag;
				m_FlagPlaced = DateTime.UtcNow;
			}
            else if (flag.Owner == m_Owner)
            {
                m_HomeFlag = flag;
            }

            Effects.PlaySound(this.Location, this.Map, 0x3D);

            int x = this.X;
            int y = this.Y;
            int z = this.Map.GetAverageZ(x, y) + 3;

            if (flag.ItemID == 0x15B6)
                y++;
            else if (flag.ItemID == 0x1627)
            {
                x++; 
                y++;
            }
            else
                x++;

			flag.MoveToWorld(new Point3D(x, y, z), this.Map);
			flag.Holder = this;
		}
		
		public void RemoveFlag(Mobile from, CTFFlag flag)
		{
			if(from.Backpack != null)
			{
                if(flag.Owner != m_Owner)
				    m_FlagPlaced = DateTime.MinValue;

                from.Backpack.DropItem(flag);
				flag.Holder = null;
			}

            Effects.PlaySound(this.Location, this.Map, 0x059);
			
			if(flag == m_HomeFlag)
				m_HomeFlag = null;
				
			if(flag == m_EnemyFlag)
				m_EnemyFlag = null;
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            if(m_Owner != null)
                list.Add(String.Format("Flag Holder for {0}", m_Owner.Name));
		}

        public override bool HandlesOnMovement { get { return m_System != null && m_System.CurrentFight != null; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_System != null && m_System.CurrentFight != null && !m.Alive && m.InRange(this.Location, 3))
            {
                TeamInfo info = m_System.CurrentFight.GetTeamInfo(m);

                if (m_Owner == null || (info != null && info.Team == m_Owner))
                {
                    m.SendGump(new ResurrectGump(m, null, ResurrectMessage.Generic, false, 0.0, Resurrect_Callback));
                }
            }
        }

        public void Resurrect_Callback(Mobile m)
        {
            ArenaKeeper.AfterResurrection(m);

            m.Hidden = true;
            m.Blessed = true;
            m.Frozen = true;
            m.PrivateOverheadMessage(Server.Network.MessageType.Regular, m.SpeechHue, false, String.Format("You are frozen for {0} more seconds!", FreezeAfterRez.ToString()), m.NetState);

            Timer.DelayCall(TimeSpan.FromSeconds(FreezeAfterRez), Unfreeze_Callback, m);
        }

        public void Unfreeze_Callback(Mobile m)
        {
            if (m != null)
            {
                m.Blessed = false;
                m.Frozen = false;
                m.PrivateOverheadMessage(Server.Network.MessageType.Regular, m.SpeechHue, false, "You can now move!", m.NetState);
            }
        }
		
		public FlagHolder(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			/*writer.Write(m_HomeFlag);
			writer.Write(m_EnemyFlag);
			writer.Write(m_TimePlaced);*/
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			/*m_HomeFlag = reader.ReadItem() as CTFFlag;
			m_EnemyFlag = reader.ReadItem() as CTFFlag;
			m_TimePlaced = reader.ReadDateTime();
			
			if(m_HomeFlag != null)
				m_HomeFlag.Holder = this;
				
			if(m_EnemyFlag != null)
				m_EnemyFlag.Holder = this;*/

            Hue = 0;
		}
	}
}