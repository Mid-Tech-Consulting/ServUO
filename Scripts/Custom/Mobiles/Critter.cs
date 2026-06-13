using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName( "a critter corpse" )]
    public class Critter : BaseCreature
    {
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );
        }

        [Constructable]
        public Critter()
        : base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4 )
        {
            Name = "Critter";
            
            Body = 130; //fire garg
            Hue = 2640;
            BaseSoundID = 0x382; //mound of maggots

            SetStr( 150, 250 );
            SetDex( 80, 125 );
            SetInt( 500, 700 );

            SetHits( 400, 600 );

            SetDamage( 13, 20 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Cold, 80 );

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 20, 45);
            SetResistance(ResistanceType.Cold, 70, 90);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 65, 85);

            SetSkill( SkillName.Magery, 100, 120 );
            SetSkill( SkillName.EvalInt, 70, 100 );
            SetSkill( SkillName.MagicResist, 110, 115 );
            SetSkill( SkillName.Tactics, 80, 100.0 );
            SetSkill( SkillName.Wrestling, 85, 100 );
            SetSkill( SkillName.Necromancy, 90, 110.0 );
            SetSkill( SkillName.SpiritSpeak, 110, 130.0 );

            Fame = 24000;
            Karma = -24000;

            PackReg( 3 );
            PackItem( new Necklace() );
        }

        public override int GetDeathSound()
        {
            return 0x370;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich );
            AddLoot( LootPack.Rich );
        }

        public override bool BleedImmune { get { return true; } }
        public override bool CanRummageCorpses { get { return false; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override bool AlwaysMurderer{ get{ return true; } }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            if( 0.1 > Utility.RandomDouble() )
            {
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if( timer != null )
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage( 1070831 ); // The freezing wind continues to blow!
                }
                else
                    defender.SendLocalizedMessage( 1070832 ); // An icy wind surrounds you, freezing your lungs as you breathe!

                timer = new ExpireTimer( defender, this );
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        private static Hashtable m_Table = new Hashtable();

        private class ExpireTimer : Timer
        {
            private Mobile m_Mobile;
            private Mobile m_From;
            private int m_Count;

            public ExpireTimer( Mobile m, Mobile from )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                Stop();
                m_Table.Remove( m_Mobile );
            }

            public void DrainLife()
            {
                if( m_Mobile.Alive )
                    m_Mobile.Damage( 2, m_From );
                else
                    DoExpire();
            }

            protected override void OnTick()
            {
                DrainLife();

                if( ++m_Count >= 5 )
                {
                    DoExpire();
                    m_Mobile.SendLocalizedMessage( 1070830 ); // The icy wind dissipates.
                }
            }
        }

        public Critter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}
