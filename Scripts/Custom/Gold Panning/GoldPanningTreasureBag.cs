//Animalcrackers//
/////////////////

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class GoldPanningTreasureBag : Item
	{

		[Constructable]
		public GoldPanningTreasureBag() : this( null )
		{
		}

		[Constructable]
      	 public GoldPanningTreasureBag(string name) : base(0xE76)  
		{
			Name = "Gold Panning Bag";
			//LootType = LootType.Cursed;
			Hue = 1161;
		}
		
		public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add("(Treasure Bag)");
        }

		public GoldPanningTreasureBag ( Serial serial ) : base ( serial )
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
                switch (Utility.Random(81))
                {
                    case 0: from.AddToBackpack(new MasksPillow()); break;
                    case 1: from.AddToBackpack(new SmallGoldNugget()); break;
					case 2: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 3: from.AddToBackpack(new AxePillow()); break;
					case 4: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 5: from.AddToBackpack(new ScalesTapestry()); break;
					case 6: from.AddToBackpack(new SwordPillow()); break;
					case 7: from.AddToBackpack(new HeartTapestry()); break;
                    case 8: from.AddToBackpack(new SmallGoldNugget()); break;
					case 9: from.AddToBackpack(new AnkhPillow()); break;
                    case 10: from.AddToBackpack(new CompassionPillow()); break;
					case 11: from.AddToBackpack(new HonestyPillow()); break;
					case 12: from.AddToBackpack(new CovetousTapestry()); break;
                    case 13: from.AddToBackpack(new SmallGoldNugget()); break;
					case 14: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 15: from.AddToBackpack(new PrideTapestry()); break;
					case 16: from.AddToBackpack(new ShieldTapestry()); break;
					case 17: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 18: from.AddToBackpack(new HumilityTapestry()); break;
					case 19: from.AddToBackpack(new HandTapestry()); break;
                    case 20: from.AddToBackpack(new JusticeTapestry()); break;
					
					case 21: from.AddToBackpack(new SmallGoldNugget()); break;
					case 22: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 23: from.AddToBackpack(new DeceitSculpture()); break;
					case 24: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 25: from.AddToBackpack(new ValorSculpture()); break;
					case 26: from.AddToBackpack(new ChaliceSculpture()); break;
					case 27: from.AddToBackpack(new HeartTapestry()); break;
                    case 28: from.AddToBackpack(new SmallGoldNugget()); break;
					case 29: from.AddToBackpack(new DespiseSculpture()); break;
                    case 30: from.AddToBackpack(new TearDropSculpture()); break;
					case 31: from.AddToBackpack(new SkullSculpture()); break;
					case 32: from.AddToBackpack(new MoonSculpture()); break;
                    case 33: from.AddToBackpack(new SmallGoldNugget()); break;
					case 34: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 35: from.AddToBackpack(new SpiritualitySculpture()); break;
					case 36: from.AddToBackpack(new ShieldTapestry()); break;
					case 37: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 38: from.AddToBackpack(new HumilityTapestry()); break;
					case 39: from.AddToBackpack(new HandTapestry()); break;
                    case 40: from.AddToBackpack(new JusticeTapestry()); break;
					
					case 41: from.AddToBackpack(new SmallGoldNugget()); break;
					case 42: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 43: from.AddToBackpack(new MediumGoldNugget()); break;
					case 44: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 45: from.AddToBackpack(new MediumGoldNugget()); break;
					case 46: from.AddToBackpack(new SmallGoldNugget()); break;
					case 47: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 48: from.AddToBackpack(new SmallGoldNugget()); break;
					case 49: from.AddToBackpack(new MediumGoldNugget()); break;
                    case 50: from.AddToBackpack(new MediumGoldNugget()); break;
					case 51: from.AddToBackpack(new MediumGoldNugget()); break;
					case 52: from.AddToBackpack(new MediumGoldNugget()); break;
                    case 53: from.AddToBackpack(new SmallGoldNugget()); break;
					case 54: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 55: from.AddToBackpack(new MediumGoldNugget()); break;
					case 56: from.AddToBackpack(new MediumGoldNugget()); break;
					case 57: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 58: from.AddToBackpack(new MediumGoldNugget()); break;
					case 59: from.AddToBackpack(new MediumGoldNugget()); break;
                    case 60: from.AddToBackpack(new MediumGoldNugget()); break;
					
					case 61: from.AddToBackpack(new TrapBag()); break;
					case 62: from.AddToBackpack(new TrapBag()); break;
                    case 63: from.AddToBackpack(new TrapBag()); break;
					case 64: from.AddToBackpack(new TrapBag()); break;
                    case 65: from.AddToBackpack(new TrapBag()); break;
					case 66: from.AddToBackpack(new SmallGoldNugget()); break;
					case 67: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 68: from.AddToBackpack(new SmallGoldNugget()); break;
					case 69: from.AddToBackpack(new LargeGoldNugget()); break;
                    case 70: from.AddToBackpack(new MediumGoldNugget()); break;
					case 71: from.AddToBackpack(new LargeGoldNugget()); break;
					case 72: from.AddToBackpack(new LargeGoldNugget()); break;
                    case 73: from.AddToBackpack(new SmallGoldNugget()); break;
					case 74: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 75: from.AddToBackpack(new MediumGoldNugget()); break;
					case 76: from.AddToBackpack(new MediumGoldNugget()); break;
					case 77: from.AddToBackpack(new SmallGoldNugget()); break;
                    case 78: from.AddToBackpack(new MediumGoldNugget()); break;
					case 79: from.AddToBackpack(new LargeGoldNugget()); break;
                    case 80: from.AddToBackpack(new MediumGoldNugget()); break;
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