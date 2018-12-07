using System;
using static GSMediator.StressTest.Net.Listener.Item.BagStruct;

namespace GSMediator.StressTest.Net.Listener.Item
{
    /// <summary>
    /// 3.1 玩家背包 + 背包數量(2) + 物品數量(2) + <<索引(2) + [sSaveItem]>>
    /// </summary>
    /// <seealso cref="GSMediator.Net.IGSocketListener" />
    public class BagListener : IGSocketListener
    {
        public void OnEvent(ref TNetMsg Msg)
        {
            ushort bagCapacity = GSocket.GetBufToWord(ref Msg.Value);
            ushort itemCount = GSocket.GetBufToWord(ref Msg.Value);

            if (itemCount > 0)
            {
                ushort ItemIdx = 0;
                sSaveItem saveItem = new sSaveItem();

                //for (int i = 1; i <= itemCount; i++)
                //{
                //    ItemIdx = GSocket.GetBufToWord(ref Msg.Value);
                //    if (GSocket.GetBufToType(ref Msg.Value, ref saveItem))
                //    {
                //        Console.WriteLine($"Item索引:{ItemIdx}, 物品ID:{saveItem.ID}");
                //    }
                //}
            }
        }
    }
}