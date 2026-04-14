//Animalcrackers//
/////////////////

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class TrapBag : Item
	{

		[Constructable]
		public TrapBag() : this( null )
		{
		}

		[Constructable]
      	public TrapBag(string name) : base(0xE76)

		{
			Name = "Trap Bag";
			//Hue = 0;
			LootType = LootType.Cursed;
		}

		public TrapBag ( Serial serial ) : base ( serial )
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
                            if (true)
                            {
                                int damage;

                                if (true)
                                    damage = Utility.RandomMinMax(5, 15);

                                AOS.Damage(from, damage, 100, 0, 0, 0, 0);
                            }
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
