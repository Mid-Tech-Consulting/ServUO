using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBAnimalTrainer : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBAnimalTrainer()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new AnimalBuyInfo(1, "a cat", typeof(Cat), 44, 10, 201, 0));
                Add(new AnimalBuyInfo(1, "a dog", typeof(Dog), 57, 10, 217, 0));
                Add(new AnimalBuyInfo(1, "a horse", typeof(Horse), 183, 10, 204, 0));
                Add(new AnimalBuyInfo(1, "a pack horse", typeof(PackHorse), 210, 10, 291, 0));
                Add(new AnimalBuyInfo(1, "a pack llama", typeof(PackLlama), 188, 10, 292, 0));
                Add(new AnimalBuyInfo(1, "a rabbit", typeof(Rabbit), 35, 10, 205, 0));

                if (!Core.AOS)
                {
                    Add(new AnimalBuyInfo(1, "an eagle", typeof(Eagle), 134, 10, 5, 0));
                    Add(new AnimalBuyInfo(1, "a brown bear", typeof(BrownBear), 285, 10, 167, 0));
                    Add(new AnimalBuyInfo(1, "a grizzly bear", typeof(GrizzlyBear), 589, 10, 212, 0));
                    Add(new AnimalBuyInfo(1, "a panther", typeof(Panther), 424, 10, 214, 0));
                    Add(new AnimalBuyInfo(1, "a timber wolf", typeof(TimberWolf), 256, 10, 225, 0));
                    Add(new AnimalBuyInfo(1, "a rat", typeof(Rat), 36, 10, 238, 0));
                }
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
}