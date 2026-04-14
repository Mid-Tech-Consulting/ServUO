// Created by Jadis
// Modified by Dawn - Admin of Götterdämmerung
// Dedicated to Jibril, who loves to be a cat
// *** 17. April 2005 ***


using System; 
using Server; 


namespace Server.Items 
{ 
   public class WarpstoneAvatarMage : Item
   { 
	    private int originalBodyHue; 
	    private int newBodyHue = 1927;
	    private int newBodyValue = 0x3CA;
		private Mobile m_Owner;

		[Constructable]
		public WarpstoneAvatarMage() : base( 0x1870 ) 
		{   Weight = 0.10; 
			Hue = 1927;
			Name = "Warpstone [Avatar Mage]"; 
			LootType = LootType.Blessed;
		} 

		public WarpstoneAvatarMage( Serial serial ) : base( serial ) 
		{ 
		} 


	   public override void OnDoubleClick( Mobile m ) 
	   { 
				
		

			// set owner if not already set -- this is only done the first time.
			if ( m_Owner == null )
			{
				m_Owner = m;
				OriginalBodyHue(m);
				this.Name = "Warpstone of " + m_Owner.Name.ToString();
				m.SendMessage( "*Your name has been engraved*" );
			}
			else
			{
				if ( m_Owner != m )
				{
					m.SendMessage( "This is not yours and will bring you no use" );
					return;
				}
				else
				{
					SwitchBody( m );
				}
					
			}
		



	   }

		public void OriginalBodyHue(Mobile m)
		{
		originalBodyHue = m.Hue;
		}

		public void SwitchBody(Mobile m)
		{
		   if (m.BodyMod == 0)
		   {
			   m.BodyMod = newBodyValue;
			   m.Hue = newBodyHue;
		   }
		   else
		   {
			   m.BodyMod = 0 ;
			   if (originalBodyHue != m.Hue)
			   {
				   m.Hue = originalBodyHue;
			   }
		   }
		  

		}

		public override void Serialize( GenericWriter writer ) 
		{ 
	
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( (int)originalBodyHue ); 
			writer.Write( (int)newBodyHue );
			writer.Write( (int)newBodyValue );
			writer.Write( (Mobile)m_Owner);

					} 
	       
		public override void Deserialize(GenericReader reader) 
		{ 
 
			base.Deserialize( reader );
			int version = reader.ReadInt();
			originalBodyHue = reader.ReadInt(); 
			newBodyHue = reader.ReadInt();
			newBodyValue = reader.ReadInt();
			m_Owner=reader.ReadMobile();	

			
		}

	   [CommandProperty( AccessLevel.GameMaster )]
	   public int originalbodyhue
	   {
		   get{ return originalBodyHue; } 
		   set{ originalBodyHue = value; }
	   }

	   [CommandProperty( AccessLevel.GameMaster )]
	   public int newbodyhue
	   {
		   get{ return newBodyHue; } 
		   set{ newBodyHue = value; }
	   }

	   

	   [CommandProperty( AccessLevel.GameMaster )]
	   public int newbodyvalue
	   {
		   get{ return newBodyValue; } 
		   set{ newBodyValue = value; }
	   }


   } 
}
