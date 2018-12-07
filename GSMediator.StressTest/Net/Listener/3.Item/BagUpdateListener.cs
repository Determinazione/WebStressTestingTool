using System;
using static GSMediator.StressTest.Net.Listener.Item.BagStruct;

namespace GSMediator.StressTest.Net.Listener.Item
{
    /// <summary>
    /// 3.2  背包變化 + Kind(1) + 索引(2) + 是否刷新(1)  
    ///                      1. 新增 + [sSaveItem]  
    ///                      2. 刪除
    /// </summary>
    /// <seealso cref="GSMediator.Net.IGSocketListener" />
    public class BagUpdateListener : IGSocketListener
    {
        public void OnEvent(ref TNetMsg Msg)
        {
            byte kind = GSocket.GetBufToByte(ref Msg.Value);
            ushort itemIdx = GSocket.GetBufToWord(ref Msg.Value);
            byte refresh = GSocket.GetBufToByte(ref Msg.Value);
            sSaveItem saveItem = new sSaveItem();

            switch (kind)
            {
                case 1:
                    if (GSocket.GetBufToType(ref Msg.Value, ref saveItem))
                    {
                        //Console.WriteLine("背包新增物品");
                        //Console.WriteLine($"Item索引:{itemIdx}, 物品ID:{saveItem.ID}");
                    }
                    break;
                case 2:
                    //Console.WriteLine("背包刪除物品");
                    //Console.WriteLine($"Item索引:{itemIdx}");
                    break;
            }

            if (refresh == 1)
            {
                //Console.WriteLine("刷新背包");
            }
        }
    }
}