using System;
using static GSMediator.StressTest.Net.Listener.Item.BagStruct;

namespace GSMediator.StressTest.Net.Listener.Item
{
    /// <summary>
    /// 3.100 送顯示用獎勵資訊 + 功能編號(1) + 是否刷新(1) + 物品總數(1) + <物品ID(2) + 物品數量(4)> 
    ///                              1. 戰鬥獎勵
    ///                              2. 副本獎勵
    ///                              3. 掃蕩獎勵                                          
    ///                              4. 事件、野怪                                    
    ///                              11. 尋訪武將
    ///                              12. 使用物品
    ///                              13. 酒館錄用
    ///                              14. 活動獎勵
    ///                              15. 活躍值獎勵
    ///                              16. 武鬥擂台獎勵
    ///                              17. 酒館錄用
    /// </summary>
    /// <seealso cref="GSMediator.Net.IGSocketListener" />
    public class DisplayRewardInfoListener : IGSocketListener
    {
        public void OnEvent(ref TNetMsg Msg)
        {
            byte kind = GSocket.GetBufToByte(ref Msg.Value);
            byte Num = GSocket.GetBufToByte(ref Msg.Value);

            sPrizeReward[] prizeAy = new sPrizeReward[Num];

            for (int i = 0; i < Num; i++)
            {
                ushort ID = GSocket.GetBufToWord(ref Msg.Value);
                uint ItemNum = GSocket.GetBufToDWord(ref Msg.Value);
                Console.WriteLine("Award ID : " + ID.ToString() + "  ItemNum : " + ItemNum.ToString());
                prizeAy[i].ItemID = ID;
                prizeAy[i].Num = ItemNum;
            }

            switch (kind)
            {
                case 1:
                    // 戰鬥獎勵
                    break;
                case 2:
                    // 副本獎勵
                    break;
                case 3:
                    //掃蕩獎勵
                    break;
                case 5: //野怪 中央訊息
                    break;
                case 11:// 尋訪武將
                    break;
                case 12:
                case 33:
                    // 使用物品
                    break;
                case 13: // 使用物品
                case 14: // 活動、成就獎勵
                case 15: // 活躍值獎勵
                case 23: // 邊境巡邏隊兌換
                case 25: // 合成獎勵
                case 26: // 副本全制霸獎勵(物品)
                case 27: // 物品信件獎勵
                case 28: // 限時購買
                case 34: // 內政收益
                case 35: // 內政(幫助獎勵)
                case 36: // 內政(掠奪獎勵)     
                         //中央訊息 訊息連跳
                    //foreach(var prize in prizeAy)
                    //{
                    //    Console.WriteLine($"中央訊息:恭喜獲得 物品ID:{prize.ItemID}, 物品數量:{prize.Num}");
                    //}
                    break;
                case 17:    // 酒館武將回禮，錄用
                    break;
                case 18:    // 七星續命燈獎勵
                    break;
                case 19:    // 熔爐獲得
                    break;
                case 20:    // 關卡全制霸獎勵
                    break;
                case 21:    // 武列全制霸獎勵(必定是武將)
                    break;
                case 22:    // 地牢錄用獎勵
                    break;
                case 24:    // 神來一翻翻牌獎勵
                    break;
                case 29:    // 首儲活動
                    break;
                case 30://採集  中央訊息
                    break;
                case 31: // 虛寶兌換
                    break;
                case 32: // 轉轉樂
                    break;
                default:
                    break;
            }
        }
    }
}