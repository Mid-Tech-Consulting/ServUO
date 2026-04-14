using System; 
using Server.Network; 
using Server.Targeting; 
using Server.Items; 

namespace Server.Items 
{
    [FlipableAttribute(0x1EFD, 0x1EFE)]
    public class BloodyShirt : BaseShirt
    {

        [Constructable] 
        public BloodyShirt() : base( 0x1EFD ) 
        { 
            Hue = 0x485; 
            Weight = 1.0; 
            Layer = Layer.MiddleTorso; 
            Name = "Bloody Shirt";
			
			Attributes.WeaponSpeed = 5;
            Attributes.DefendChance = 5;
            Attributes.BonusStr = 5;
        } 
		public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add("(Event Item)");
        }


        public BloodyShirt( Serial serial ) : base( serial ) 
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