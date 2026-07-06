using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBRanger : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRanger()
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
                Add(new AnimalBuyInfo(1, "a cat", typeof(Cat), 46, 20, 201, 0));
                Add(new AnimalBuyInfo(1, "a dog", typeof(Dog), 60, 20, 217, 0));
                Add(new AnimalBuyInfo(1, "a pack llama", typeof(PackLlama), 163, 20, 292, 0));
                Add(new AnimalBuyInfo(1, "a pack horse", typeof(PackHorse), 202, 20, 291, 0));
                Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0, true));
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