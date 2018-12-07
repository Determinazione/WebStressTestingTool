using GSMediator.StressTest.PerformaceMonitor;
using System;
using System.Threading;

namespace GSMediator.StressTest.Net.Listener.Connect
{
    public class SignupListener : IGSocketListener
    {
        private readonly GSocket _socket;
        private readonly MonitorUnit _monitorUnit;
        public SignupListener(GSocket socket, MonitorUnit monitor)
        {
            _socket = socket;
            _monitorUnit = monitor;
        }

        public void OnEvent(ref TNetMsg Msg)
        {
            byte kind = GSocket.GetBufToByte(ref Msg.Value);
            Console.WriteLine("創角結果(kind):" + kind);
        }
    }
}