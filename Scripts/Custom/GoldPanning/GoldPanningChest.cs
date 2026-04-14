//Animalcrackers//
/////////////////

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class GoldPanningTreasureChest : Item
	{

		[Constructable]
		public GoldPanningTreasureChest() : this( null )
		{
		}

		[Constructable]
    	        public GoldPanningTreasureChest(string name) : base(0xE41)
           
		{
			Name = "<BASEFONT COLOR=#FFC0CB>Treasure Chest [Gold Panning]</font>";
			//Name = "Treasure Chest [Archeology]";
			Hue = 0;
		}
		public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add("(Treasure Chest)");
        }

		public GoldPanningTreasureChest ( Serial serial ) : base ( serial )
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
				 //double chance = Utility.RandomDouble();

            //if (chance < 0.001) // 9% chance for drop
            //{
            //switch (Utility.Random(3))
            //{
                 //case 0: from.AddToBackpack(new ArcheologyOmegaRewardBag()); break;
                 //case 1: from.AddToBackpack(new ArcheologyOmegaRewardBag()); break;
            //}
            //}
                switch (Utility.Random(150))
                {
                    case 0: from.AddToBackpack(new EternalFlame()); break;
                    case 1: from.AddToBackpack(new BlazeStaff()); break;
                    case 2: from.AddToBackpack(new BloodyShirt()); break;
                    case 3: from.AddToBackpack(new WarpstoneAvatar()); break;
					case 4: from.AddToBackpack(new QuiverOfTheGods());
							from.AddToBackpack(new GoldGlasses()); break;
					case 5: from.AddToBackpack(new GoldBrick()); break;
                    case 6: from.AddToBackpack(new SaltyBoots()); break;
                    case 7: from.AddToBackpack(new GoldMinersApron());
							from.AddToBackpack(new WarpstoneAvatarMystic()); break;
                    case 8: from.AddToBackpack(new WarpstoneAvatarMage()); break;
					case 9: from.AddToBackpack(new WarpstoneAvatar());
							from.AddToBackpack(new EtherealShroud());
							from.AddToBackpack(new FortuneseekerTreads()); break;
							
					case 10: from.AddToBackpack(new ArachnidSlayerDeed()); break;
                    case 11: from.AddToBackpack(new DemonSlayerDeed()); break;
                    case 12: from.AddToBackpack(new ElementalSlayerDeed()); break;
                    case 13: from.AddToBackpack(new FeySlayerDeed()); break;
					case 14: from.AddToBackpack(new RepondSlayerDeed());
							from.AddToBackpack(new ReptileSlayerDeed()); break;
					case 15: from.AddToBackpack(new UndeadSlayerDeed()); break;
                    case 16: from.AddToBackpack(new PetResurrectionDeed()); break;
                    case 17: from.AddToBackpack(new OldDirtySash());
							from.AddToBackpack(new GoldBrick()); break;
                    case 18: from.AddToBackpack(new GoldBrick()); break;
					case 19: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 20: from.AddToBackpack(new GoldBrick()); break;
                    case 21: from.AddToBackpack(new GoldBrick()); break;
                    case 22: from.AddToBackpack(new GoldBrick()); break;
                    case 23: from.AddToBackpack(new GoldBrick()); break;
					case 24: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 25: from.AddToBackpack(new GoldBrick()); break;
                    case 26: from.AddToBackpack(new GoldPan()); break;
                    case 27: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 28: from.AddToBackpack(new GoldBrick()); break;
					case 29: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 30: from.AddToBackpack(new GoldBrick()); break;
                    case 31: from.AddToBackpack(new GoldBrick()); break;
                    case 32: from.AddToBackpack(new GoldBrick()); break;
                    case 33: from.AddToBackpack(new GoldBrick()); break;
					case 34: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 35: from.AddToBackpack(new GoldBrick()); break;
                    case 36: from.AddToBackpack(new GoldPan()); break;
                    case 37: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 38: from.AddToBackpack(new GoldBrick()); break;
					case 39: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 40: from.AddToBackpack(new GoldBrick()); break;
                    case 41: from.AddToBackpack(new GoldBrick()); break;
                    case 42: from.AddToBackpack(new GoldBrick()); break;
                    case 43: from.AddToBackpack(new GoldBrick()); break;
					case 44: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 45: from.AddToBackpack(new GoldBrick()); break;
                    case 46: from.AddToBackpack(new GoldPan()); break;
                    case 47: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 48: from.AddToBackpack(new GoldBrick()); break;
					case 49: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 50: from.AddToBackpack(new GoldBrick()); break;
                    case 51: from.AddToBackpack(new GoldBrick()); break;
                    case 52: from.AddToBackpack(new GoldBrick()); break;
                    case 53: from.AddToBackpack(new GoldBrick()); break;
					case 54: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 55: from.AddToBackpack(new GoldBrick()); break;
                    case 56: from.AddToBackpack(new GoldPan()); break;
                    case 57: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 58: from.AddToBackpack(new GoldBrick()); break;
					case 59: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 60: from.AddToBackpack(new GoldBrick()); break;
                    case 61: from.AddToBackpack(new GoldBrick()); break;
                    case 62: from.AddToBackpack(new GoldBrick()); break;
                    case 63: from.AddToBackpack(new GoldBrick()); break;
					case 64: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 65: from.AddToBackpack(new GoldBrick()); break;
                    case 66: from.AddToBackpack(new GoldPan()); break;
                    case 67: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 68: from.AddToBackpack(new GoldBrick()); break;
					case 69: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 70: from.AddToBackpack(new GoldBrick()); break;
                    case 71: from.AddToBackpack(new GoldBrick()); break;
                    case 72: from.AddToBackpack(new GoldBrick()); break;
                    case 73: from.AddToBackpack(new GoldBrick()); break;
					case 74: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 75: from.AddToBackpack(new GoldBrick()); break;
                    case 76: from.AddToBackpack(new GoldPan()); break;
                    case 77: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 78: from.AddToBackpack(new GoldBrick()); break;
					case 79: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 80: from.AddToBackpack(new GoldBrick()); break;
                    case 81: from.AddToBackpack(new GoldBrick()); break;
                    case 82: from.AddToBackpack(new GoldBrick()); break;
                    case 83: from.AddToBackpack(new GoldBrick()); break;
					case 84: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 85: from.AddToBackpack(new GoldBrick()); break;
                    case 86: from.AddToBackpack(new GoldBrick()); break;
                    case 87: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 88: from.AddToBackpack(new GoldBrick()); break;
					case 89: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 90: from.AddToBackpack(new GoldBrick()); break;
                    case 91: from.AddToBackpack(new GoldBrick()); break;
                    case 92: from.AddToBackpack(new GoldBrick()); break;
                    case 93: from.AddToBackpack(new GoldBrick()); break;
					case 94: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 95: from.AddToBackpack(new GoldBrick()); break;
                    case 96: from.AddToBackpack(new GoldBrick()); break;
                    case 97: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 98: from.AddToBackpack(new GoldBrick()); break;
					case 99: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 100: from.AddToBackpack(new GoldBrick()); break;
                    case 101: from.AddToBackpack(new GoldBrick()); break;
                    case 102: from.AddToBackpack(new GoldBrick()); break;
                    case 103: from.AddToBackpack(new GoldBrick()); break;
					case 104: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 105: from.AddToBackpack(new GoldBrick()); break;
                    case 106: from.AddToBackpack(new GoldBrick()); break;
                    case 107: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 108: from.AddToBackpack(new GoldBrick()); break;
					case 109: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 110: from.AddToBackpack(new GoldBrick()); break;
                    case 111: from.AddToBackpack(new GoldBrick()); break;
                    case 112: from.AddToBackpack(new GoldBrick()); break;
                    case 113: from.AddToBackpack(new GoldBrick()); break;
					case 114: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 115: from.AddToBackpack(new GoldBrick()); break;
                    case 116: from.AddToBackpack(new GoldBrick()); break;
                    case 117: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 118: from.AddToBackpack(new GoldBrick()); break;
					case 119: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 120: from.AddToBackpack(new GoldBrick()); break;
                    case 121: from.AddToBackpack(new GoldBrick()); break;
                    case 122: from.AddToBackpack(new GoldBrick()); break;
                    case 123: from.AddToBackpack(new GoldBrick()); break;
					case 124: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 125: from.AddToBackpack(new GoldBrick()); break;
                    case 126: from.AddToBackpack(new GoldBrick()); break;
                    case 127: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 128: from.AddToBackpack(new GoldBrick()); break;
					case 129: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
							
					case 130: from.AddToBackpack(new GoldBrick()); break;
                    case 131: from.AddToBackpack(new GoldBrick()); break;
                    case 132: from.AddToBackpack(new GoldBrick()); break;
                    case 133: from.AddToBackpack(new GoldBrick()); break;
					case 134: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 135: from.AddToBackpack(new GoldBrick()); break;
                    case 136: from.AddToBackpack(new GoldBrick()); break;
                    case 137: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 138: from.AddToBackpack(new GoldBrick()); break;
					case 139: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					
					case 140: from.AddToBackpack(new GoldBrick()); break;
                    case 141: from.AddToBackpack(new GoldBrick()); break;
                    case 142: from.AddToBackpack(new GoldBrick()); break;
                    case 143: from.AddToBackpack(new GoldBrick()); break;
					case 144: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
					case 145: from.AddToBackpack(new GoldBrick()); break;
                    case 146: from.AddToBackpack(new GoldBrick()); break;
                    case 147: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
                    case 148: from.AddToBackpack(new GoldBrick()); break;
					case 149: from.AddToBackpack(new GoldBrick());
							from.AddToBackpack(new GoldBrick()); break;
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