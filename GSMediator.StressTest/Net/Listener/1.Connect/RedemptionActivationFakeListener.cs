using GSMediator.StressTest.PerformaceMonitor;
using System;

namespace GSMediator.StressTest.Net.Listener.Connect
{
    internal class RedemptionActivationFakeListener : IGSocketListener
    {
        private const byte SUCCESS = 1;
        private const byte HTTP_FAILED = 2;
        private const byte BAG_IS_FULL = 3;
        private const byte ADD_PRIZE_FAILED = 4;

        private readonly MonitorUnit _monitorUnit;
        public RedemptionActivationFakeListener(MonitorUnit monitor)
        {
            _monitorUnit = monitor;
        }

        public void OnEvent(ref TNetMsg Msg)
        {
            var kind = GSocket.GetBufToByte(ref Msg.Value);
            switch (kind)
            {
                case SUCCESS:
                    // 關閉效能監測工具，並儲存此次紀錄結果
                    _monitorUnit.StopAndSaveToMonitorManager(true, "禮包碼兌換成功");
                    Console.WriteLine($"{_monitorUnit.GetTag()}禮包碼兌換成功");
                    break;
                case HTTP_FAILED:
                    _monitorUnit.StopAndSaveToMonitorManager(false, "禮包碼兌換失敗");
                    Console.WriteLine($"{_monitorUnit.GetTag()}禮包碼兌換失敗");
                    break;
                case BAG_IS_FULL:
                    _monitorUnit.StopAndSaveToMonitorManager(false, "背包已滿");
                    Console.WriteLine($"{_monitorUnit.GetTag()}背包已滿");
                    break;
                case ADD_PRIZE_FAILED:
                    _monitorUnit.StopAndSaveToMonitorManager(false, "新增禮包碼到背包中失敗");
                    Console.WriteLine($"{_monitorUnit.GetTag()}新增禮包碼到背包中失敗");
                    break;
            }
        }
    }
}