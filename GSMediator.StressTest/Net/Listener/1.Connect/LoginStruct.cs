using System;

namespace GSMediator.StressTest.Net.Listener.Connect
{
    public class LoginStruct
    {
        public struct sLoginType
        {
            public uint ManID;

            [Myatt(16)]
            public string Name;

            public byte Sex;
            public byte Arms;
            public uint GoldMoney;
            public uint FakeMoney;
            public uint Money;

            /// <summary>
            /// 鍛造值
            /// </summary>
            public uint GrowItemValue;

            public uint ExpPool;            //額外經驗槽
            public sLv1nExp4 Vip;

            /// <summary>
            /// 紅利
            /// </summary>
            public uint Bonus;

            /// <summary>
            /// 消費累積
            /// </summary>
            public byte TotalCashAdd;    //消費累計(最多100)

            /// <summary>
            /// 武鬥徽章
            /// </summary>
            public uint PKCoin;

            /// <summary>
            /// 榮譽徽章
            /// </summary>
            public uint HonorCoin;

            /// <summary>
            /// 活動徽章
            /// </summary>
            public uint ActivityCoin;

            /// <summary>
            /// 魂石
            /// </summary>
            public uint SoulStone;

            public ushort GuildID; //公會ID(大於0有軍團)
            public byte GuildKind;  //公會陣營(0.無 1.義軍 2.山寨)

            public uint SkillPoint;

            public uint GuildMoney;
            public uint CampValue;//陣營軍功
            public uint GuildWarValue; //軍團戰積分
            public ushort KillNum;     //軍團戰殺人數

            /// <summary>
            /// 帳號
            /// </summary>
            [Myatt(64)]
            public string Account;

            /// <summary>
            /// FB綁定
            /// </summary>
            [Myatt(32)]
            public string FBBindId;

            /// <summary>
            /// 創角時間
            /// </summary>
            public DateTime BornTime;

            public uint Reputation; // 內政名聲

            public sLoginType(bool flag)
            {
                ManID = 0;
                Sex = 0;
                Arms = 0;
                GoldMoney = 0;
                FakeMoney = 0;
                Money = 0;
                GrowItemValue = 0;
                ExpPool = 0;
                Name = string.Empty;
                Vip = new sLv1nExp4();
                Bonus = 0;
                TotalCashAdd = 0;
                PKCoin = 0;
                HonorCoin = 0;
                ActivityCoin = 0;
                SoulStone = 0;
                GuildID = 0;
                GuildKind = 0;
                SkillPoint = 0;
                GuildMoney = 0;
                CampValue = 0;
                GuildWarValue = 0;
                KillNum = 0;
                Account = string.Empty;
                FBBindId = string.Empty;
                BornTime = DateTime.Today;
                Reputation = 0;
            }
        }

        public struct sLv1nExp4
        {
            public byte Lv;
            public uint Exp;
        }
    }
}