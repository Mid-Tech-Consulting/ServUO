using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	
	public class OldDirtySash : BaseMiddleTorso
	{

		[Constructable]
		public OldDirtySash() : base( 0x1541 )
		{
						Name = "<BASEFONT COLOR=#FF8000>Old Dirty Sash [OMEGA]</font>";
						Hue = 43;
						Weight = 1.0;

						Attributes.SpellDamage = 10;
						Resistances.Physical = 10;


                       int val = Utility.RandomList(5);
                       switch ( Utility.Random( 54 ) )   
			{
		                   
		                case 0: SkillBonuses.SetValues( 0, SkillName.Alchemy,  val  ); break;
				case 1: SkillBonuses.SetValues( 0, SkillName.Anatomy, val ); break;
				case 2: SkillBonuses.SetValues( 0, SkillName.AnimalLore, val ); break;
                                case 3: SkillBonuses.SetValues( 0, SkillName.ItemID, val ); break;
				case 4: SkillBonuses.SetValues( 0, SkillName.ArmsLore, val ); break;
                                case 5: SkillBonuses.SetValues( 0, SkillName.Parry, val ); break;
                                case 6: SkillBonuses.SetValues( 0, SkillName.Begging, val ); break;
				case 7: SkillBonuses.SetValues( 0, SkillName.Blacksmith, val ); break;
				case 8: SkillBonuses.SetValues( 0, SkillName.Fletching, val ); break;
                                case 9: SkillBonuses.SetValues( 0, SkillName.Peacemaking , val ); break;
				case 10: SkillBonuses.SetValues( 0,SkillName.Camping, val ); break;
				case 11: SkillBonuses.SetValues( 0,SkillName.Carpentry, val ); break;
                                case 12: SkillBonuses.SetValues( 0,SkillName.Cartography, val ); break;
				case 13: SkillBonuses.SetValues( 0,SkillName.Cooking, val ); break;
				case 14: SkillBonuses.SetValues( 0,SkillName.DetectHidden, val ); break;
                                case 15: SkillBonuses.SetValues( 0,SkillName.Discordance, val ); break;
				case 16: SkillBonuses.SetValues( 0,SkillName.EvalInt, val ); break;
				case 17: SkillBonuses.SetValues( 0,SkillName.Healing, val ); break;
                                case 18: SkillBonuses.SetValues( 0,SkillName.Fishing, val ); break;
				case 19: SkillBonuses.SetValues( 0,SkillName.Forensics, val ); break;
				case 20: SkillBonuses.SetValues( 0,SkillName.Herding, val ); break;
                                case 21: SkillBonuses.SetValues( 0,SkillName.Hiding, val ); break;
				case 22: SkillBonuses.SetValues( 0,SkillName.Provocation, val ); break;
				case 23: SkillBonuses.SetValues( 0,SkillName.Inscribe, val ); break;
                                case 24: SkillBonuses.SetValues( 0,SkillName.Lockpicking, val ); break;
				case 25: SkillBonuses.SetValues( 0,SkillName.Magery, val ); break;
				case 26: SkillBonuses.SetValues( 0,SkillName.MagicResist, val ); break;
                                case 27: SkillBonuses.SetValues( 0,SkillName.Tactics, val ); break;
				case 28: SkillBonuses.SetValues( 0,SkillName.Snooping, val ); break;
				case 29: SkillBonuses.SetValues( 0,SkillName.Musicianship, val ); break;
                                case 30: SkillBonuses.SetValues( 0,SkillName.Poisoning, val ); break;
				case 31: SkillBonuses.SetValues( 0,SkillName.Archery, val ); break;
				case 32: SkillBonuses.SetValues( 0,SkillName.SpiritSpeak, val ); break;
                                case 33: SkillBonuses.SetValues( 0,SkillName.Stealing, val ); break;
				case 34: SkillBonuses.SetValues( 0,SkillName.Tailoring, val ); break;
				case 35: SkillBonuses.SetValues( 0,SkillName.AnimalTaming, val ); break;
                                case 36: SkillBonuses.SetValues( 0,SkillName.TasteID, val ); break;
				case 37: SkillBonuses.SetValues( 0,SkillName.Tinkering, val ); break;
				case 38: SkillBonuses.SetValues( 0,SkillName.Tracking , val ); break;
                                case 39: SkillBonuses.SetValues( 0,SkillName.Veterinary, val ); break;
				case 40: SkillBonuses.SetValues( 0,SkillName.Swords, val ); break;
				case 41: SkillBonuses.SetValues( 0,SkillName.Macing, val ); break;
                                case 42: SkillBonuses.SetValues( 0,SkillName.Fencing, val ); break;
				case 43: SkillBonuses.SetValues( 0,SkillName.Wrestling, val ); break;
				case 44: SkillBonuses.SetValues( 0,SkillName.Lumberjacking, val ); break;
                                case 45: SkillBonuses.SetValues( 0,SkillName.Mining, val ); break;
				case 46: SkillBonuses.SetValues( 0,SkillName.Meditation, val ); break;
				case 47: SkillBonuses.SetValues( 0,SkillName.Stealth, val ); break;
                                case 48: SkillBonuses.SetValues( 0,SkillName.RemoveTrap, val ); break;
				case 49: SkillBonuses.SetValues( 0,SkillName.Necromancy , val ); break;
				case 50: SkillBonuses.SetValues( 0,SkillName.Focus, val ); break;
                                case 51: SkillBonuses.SetValues( 0,SkillName.Chivalry, val ); break;
				case 52: SkillBonuses.SetValues( 0,SkillName.Bushido, val ); break;
				case 53: SkillBonuses.SetValues( 0,SkillName.Ninjitsu, val ); break;
                          
            }
						 int val2 = Utility.RandomList(5);
                       switch ( Utility.Random( 54 ) )   
			{
		                   
		                case 0: SkillBonuses.SetValues( 1, SkillName.Alchemy,  val2  ); break;
				case 1: SkillBonuses.SetValues( 1, SkillName.Anatomy, val2 ); break;
				case 2: SkillBonuses.SetValues( 1, SkillName.AnimalLore, val2 ); break;
                                case 3: SkillBonuses.SetValues( 1, SkillName.ItemID, val2 ); break;
				case 4: SkillBonuses.SetValues( 1, SkillName.ArmsLore, val2 ); break;
                                case 5: SkillBonuses.SetValues( 1, SkillName.Parry, val2 ); break;
                                case 6: SkillBonuses.SetValues( 1, SkillName.Begging, val2 ); break;
				case 7: SkillBonuses.SetValues( 1, SkillName.Blacksmith, val2 ); break;
				case 8: SkillBonuses.SetValues( 1, SkillName.Fletching, val2 ); break;
                                case 9: SkillBonuses.SetValues( 1, SkillName.Peacemaking , val2 ); break;
				case 10: SkillBonuses.SetValues( 1,SkillName.Camping, val2 ); break;
				case 11: SkillBonuses.SetValues( 1,SkillName.Carpentry, val2 ); break;
                                case 12: SkillBonuses.SetValues( 1,SkillName.Cartography, val2 ); break;
				case 13: SkillBonuses.SetValues( 1,SkillName.Cooking, val2 ); break;
				case 14: SkillBonuses.SetValues( 1,SkillName.DetectHidden, val2 ); break;
                                case 15: SkillBonuses.SetValues( 1,SkillName.Discordance, val2 ); break;
				case 16: SkillBonuses.SetValues( 1,SkillName.EvalInt, val2 ); break;
				case 17: SkillBonuses.SetValues( 1,SkillName.Healing, val2 ); break;
                                case 18: SkillBonuses.SetValues( 1,SkillName.Fishing, val2 ); break;
				case 19: SkillBonuses.SetValues( 1,SkillName.Forensics, val2 ); break;
				case 20: SkillBonuses.SetValues( 1,SkillName.Herding, val2 ); break;
                                case 21: SkillBonuses.SetValues( 1,SkillName.Hiding, val2 ); break;
				case 22: SkillBonuses.SetValues( 1,SkillName.Provocation, val2 ); break;
				case 23: SkillBonuses.SetValues( 1,SkillName.Inscribe, val2 ); break;
                                case 24: SkillBonuses.SetValues( 1,SkillName.Lockpicking, val2 ); break;
				case 25: SkillBonuses.SetValues( 1,SkillName.Magery, val2 ); break;
				case 26: SkillBonuses.SetValues( 1,SkillName.MagicResist, val2 ); break;
                                case 27: SkillBonuses.SetValues( 1,SkillName.Tactics, val2 ); break;
				case 28: SkillBonuses.SetValues( 1,SkillName.Snooping, val2 ); break;
				case 29: SkillBonuses.SetValues( 1,SkillName.Musicianship, val2 ); break;
                                case 30: SkillBonuses.SetValues( 1,SkillName.Poisoning, val2 ); break;
				case 31: SkillBonuses.SetValues( 1,SkillName.Archery, val2 ); break;
				case 32: SkillBonuses.SetValues( 1,SkillName.SpiritSpeak, val2 ); break;
                                case 33: SkillBonuses.SetValues( 1,SkillName.Stealing, val2 ); break;
				case 34: SkillBonuses.SetValues( 1,SkillName.Tailoring, val2 ); break;
				case 35: SkillBonuses.SetValues( 1,SkillName.AnimalTaming, val2 ); break;
                                case 36: SkillBonuses.SetValues( 1,SkillName.TasteID, val2 ); break;
				case 37: SkillBonuses.SetValues( 1,SkillName.Tinkering, val2 ); break;
				case 38: SkillBonuses.SetValues( 1,SkillName.Tracking , val2 ); break;
                                case 39: SkillBonuses.SetValues( 1,SkillName.Veterinary, val2 ); break;
				case 40: SkillBonuses.SetValues( 1,SkillName.Swords, val2 ); break;
				case 41: SkillBonuses.SetValues( 1,SkillName.Macing, val2 ); break;
                                case 42: SkillBonuses.SetValues( 1,SkillName.Fencing, val2 ); break;
				case 43: SkillBonuses.SetValues( 1,SkillName.Wrestling, val2 ); break;
				case 44: SkillBonuses.SetValues( 1,SkillName.Lumberjacking, val2 ); break;
                                case 45: SkillBonuses.SetValues( 1,SkillName.Mining, val2 ); break;
				case 46: SkillBonuses.SetValues( 1,SkillName.Meditation, val2 ); break;
				case 47: SkillBonuses.SetValues( 1,SkillName.Stealth, val2 ); break;
                                case 48: SkillBonuses.SetValues( 1,SkillName.RemoveTrap, val2 ); break;
				case 49: SkillBonuses.SetValues( 1,SkillName.Necromancy , val2 ); break;
				case 50: SkillBonuses.SetValues( 1,SkillName.Focus, val2 ); break;
                                case 51: SkillBonuses.SetValues( 1,SkillName.Chivalry, val2 ); break;
				case 52: SkillBonuses.SetValues( 1,SkillName.Bushido, val2 ); break;
				case 53: SkillBonuses.SetValues( 1,SkillName.Ninjitsu, val2 ); break;
                          
                         }
						 int val3 = Utility.RandomList(5);
                       switch ( Utility.Random( 54 ) )   
			{
		                   
		                case 0: SkillBonuses.SetValues( 2, SkillName.Alchemy,  val3  ); break;
				case 1: SkillBonuses.SetValues( 2, SkillName.Anatomy, val3 ); break;
				case 2: SkillBonuses.SetValues( 2, SkillName.AnimalLore, val3 ); break;
                                case 3: SkillBonuses.SetValues( 2, SkillName.ItemID, val3 ); break;
				case 4: SkillBonuses.SetValues( 2, SkillName.ArmsLore, val3 ); break;
                                case 5: SkillBonuses.SetValues( 2, SkillName.Parry, val3 ); break;
                                case 6: SkillBonuses.SetValues( 2, SkillName.Begging, val3 ); break;
				case 7: SkillBonuses.SetValues( 2, SkillName.Blacksmith, val3 ); break;
				case 8: SkillBonuses.SetValues( 2, SkillName.Fletching, val3 ); break;
                                case 9: SkillBonuses.SetValues( 2, SkillName.Peacemaking , val3 ); break;
				case 10: SkillBonuses.SetValues( 2,SkillName.Camping, val3 ); break;
				case 11: SkillBonuses.SetValues( 2,SkillName.Carpentry, val3 ); break;
                                case 12: SkillBonuses.SetValues( 2,SkillName.Cartography, val3 ); break;
				case 13: SkillBonuses.SetValues( 2,SkillName.Cooking, val3 ); break;
				case 14: SkillBonuses.SetValues( 2,SkillName.DetectHidden, val3 ); break;
                                case 15: SkillBonuses.SetValues( 2,SkillName.Discordance, val3 ); break;
				case 16: SkillBonuses.SetValues( 2,SkillName.EvalInt, val3 ); break;
				case 17: SkillBonuses.SetValues( 2,SkillName.Healing, val3 ); break;
                                case 18: SkillBonuses.SetValues( 2,SkillName.Fishing, val3 ); break;
				case 19: SkillBonuses.SetValues( 2,SkillName.Forensics, val3 ); break;
				case 20: SkillBonuses.SetValues( 2,SkillName.Herding, val3 ); break;
                                case 21: SkillBonuses.SetValues( 2,SkillName.Hiding, val3 ); break;
				case 22: SkillBonuses.SetValues( 2,SkillName.Provocation, val3 ); break;
				case 23: SkillBonuses.SetValues( 2,SkillName.Inscribe, val3 ); break;
                                case 24: SkillBonuses.SetValues( 2,SkillName.Lockpicking, val3 ); break;
				case 25: SkillBonuses.SetValues( 2,SkillName.Magery, val3 ); break;
				case 26: SkillBonuses.SetValues( 2,SkillName.MagicResist, val3 ); break;
                                case 27: SkillBonuses.SetValues( 2,SkillName.Tactics, val3 ); break;
				case 28: SkillBonuses.SetValues( 2,SkillName.Snooping, val3 ); break;
				case 29: SkillBonuses.SetValues( 2,SkillName.Musicianship, val3 ); break;
                                case 30: SkillBonuses.SetValues( 2,SkillName.Poisoning, val3 ); break;
				case 31: SkillBonuses.SetValues( 2,SkillName.Archery, val3 ); break;
				case 32: SkillBonuses.SetValues( 2,SkillName.SpiritSpeak, val3 ); break;
                                case 33: SkillBonuses.SetValues( 2,SkillName.Stealing, val3 ); break;
				case 34: SkillBonuses.SetValues( 2,SkillName.Tailoring, val3 ); break;
				case 35: SkillBonuses.SetValues( 2,SkillName.AnimalTaming, val3 ); break;
                                case 36: SkillBonuses.SetValues( 2,SkillName.TasteID, val3 ); break;
				case 37: SkillBonuses.SetValues( 2,SkillName.Tinkering, val3 ); break;
				case 38: SkillBonuses.SetValues( 2,SkillName.Tracking , val3 ); break;
                                case 39: SkillBonuses.SetValues( 2,SkillName.Veterinary, val3 ); break;
				case 40: SkillBonuses.SetValues( 2,SkillName.Swords, val3 ); break;
				case 41: SkillBonuses.SetValues( 2,SkillName.Macing, val3 ); break;
                                case 42: SkillBonuses.SetValues( 2,SkillName.Fencing, val3 ); break;
				case 43: SkillBonuses.SetValues( 2,SkillName.Wrestling, val3 ); break;
				case 44: SkillBonuses.SetValues( 2,SkillName.Lumberjacking, val3 ); break;
                                case 45: SkillBonuses.SetValues( 2,SkillName.Mining, val3 ); break;
				case 46: SkillBonuses.SetValues( 2,SkillName.Meditation, val3 ); break;
				case 47: SkillBonuses.SetValues( 2,SkillName.Stealth, val3 ); break;
                                case 48: SkillBonuses.SetValues( 2,SkillName.RemoveTrap, val3 ); break;
				case 49: SkillBonuses.SetValues( 2,SkillName.Necromancy , val3 ); break;
				case 50: SkillBonuses.SetValues( 2,SkillName.Focus, val3 ); break;
                                case 51: SkillBonuses.SetValues( 2,SkillName.Chivalry, val3 ); break;
				case 52: SkillBonuses.SetValues( 2,SkillName.Bushido, val3 ); break;
				case 53: SkillBonuses.SetValues( 2,SkillName.Ninjitsu, val3 ); break;
                          
                         }

                }

		public OldDirtySash( Serial serial ) : base( serial )
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