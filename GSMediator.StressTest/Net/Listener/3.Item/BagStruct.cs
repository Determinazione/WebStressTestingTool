namespace GSMediator.StressTest.Net.Listener.Item
{
    public class BagStruct
    {
        public struct sSaveItem
        {
            /// <summary>
            /// 物品ID
            /// </summary>
            public ushort ID;

            /// <summary>
            /// KIND11～25裝備次數 / 物品堆疊
            /// </summary>
            public byte NumStack;

            /// <summary>
            /// 交易狀態(暫定)
            /// </summary>
            public byte State;

            /// <summary>
            /// 基礎數值
            /// 陣法用: 0 = 品階 1 = 經驗
            /// </summary>
            [Myatt(2)]
            public ushort[] AttrAy;

            /// <summary>
            /// 隨機屬性
            /// </summary>
            [Myatt(4)]
            public sAddAttr[] EffectAy;

            /// <summary>
            /// 裝備目前裝備在的武將身上
            /// </summary>
            public byte UseIdx;

            /// <summary>
            /// 強化等級
            /// </summary>
            public byte GrowLv;

            /// <summary>
            /// 流水號
            /// </summary>
            public uint SerialNo;

            /// <summary>
            /// 星等
            /// </summary>
            public byte Rank;

            [Myatt(23)]
            public byte[] tmp;

            public sSaveItem(bool flag)
            {
                ID = 0;
                NumStack = 0;
                State = 0;
                AttrAy = new ushort[2];
                EffectAy = new sAddAttr[4];
                UseIdx = byte.MaxValue;
                GrowLv = 0;
                SerialNo = 0;
                Rank = 0;
                tmp = new byte[23];
            }
        }

        public struct sAddAttr
        {
            /// <summary>
            /// 數值種類
            /// </summary>
            public byte Kind;

            /// <summary>
            /// 數值
            /// </summary>
            public ushort Value;
        }

        public struct sPrizeReward
        {
            public ushort ItemID;
            public uint Num;

            public sPrizeReward(bool flag)
            {
                ItemID = 0;
                Num = 0;
            }
        }
    }
}