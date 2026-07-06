using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBVeterinarian : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBVeterinarian()
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
                Add(new GenericBuyInfo(typeof(Bandage), 6, 20, 0xE21, 0, true));
                Add(new AnimalBuyInfo(1, "a pack horse", typeof(PackHorse), 205, 10, 291, 0));
                Add(new AnimalBuyInfo(1, "a pack llama", typeof(PackLlama), 174, 10, 292, 0));
                Add(new AnimalBuyInfo(1, "a dog", typeof(Dog), 52, 10, 217, 0));
                Add(new AnimalBuyInfo(1, "a cat", typeof(Cat), 43, 10, 201, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Bandage), 1);
            }
        }
    }
}