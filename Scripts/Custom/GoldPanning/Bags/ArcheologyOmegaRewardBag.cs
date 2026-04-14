//Animalcrackers//
/////////////////

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class ArcheologyOmegaRewardBag : Item
	{

		[Constructable]
		public ArcheologyOmegaRewardBag() : this( null )
		{
		}

		[Constructable]
      	        public ArcheologyOmegaRewardBag(string name) : base(0xE76)
           
		{
			Name = "<BASEFONT COLOR=#FF8000>Archeology Reward Bag [OMEGA]</font>";
			LootType = LootType.Cursed;
			Hue = 43;
		}

		public ArcheologyOmegaRewardBag ( Serial serial ) : base ( serial )
		{
		}

      		public override void OnDoubleClick( Mobile from ) 
      		{
			if ( !IsChildOf( from.Backpack ) )
			{
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                switch (Utility.Random(5))
                {
                    case 0: from.AddToBackpack(new OldDirtySash()); break;
                    case 1: from.AddToBackpack(new OldDirtySash()); break;
                    case 2: from.AddToBackpack(new OldDirtySash()); break;
					case 3: from.AddToBackpack(new OldDirtySash()); break;
                    case 4: from.AddToBackpack(new OldDirtySash()); break;
                }
                this.Delete();
			}

		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}