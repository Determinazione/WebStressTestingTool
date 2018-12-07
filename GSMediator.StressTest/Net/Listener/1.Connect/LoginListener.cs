using GSMediator.StressTest.PerformaceMonitor;
using GSMediator.StressTest.Generator;
using System;
using System.Threading;
using static GSMediator.StressTest.Net.Listener.Connect.LoginStruct;

namespace GSMediator.StressTest.Net.Listener.Connect
{
    public class LoginListener : IGSocketListener
    {
        private readonly GSocket _socket;
        private readonly MonitorUnit _monitorUnit;
        public LoginListener(GSocket socket, MonitorUnit monitor)
        {
            _socket = socket;
            _monitorUnit = monitor;

        }

        public void OnEvent(ref TNetMsg Msg)
        {
            byte kind = GSocket.GetBufToByte(ref Msg.Value);
            //Console.WriteLine("登入結果(kind):" + kind);
            byte sex = 0;
            byte armType = 0;

            switch (kind)
            {
                case 1:
                    // 登入成功
                    var gsid = GSocket.GetBufToWord(ref Msg.Value);
                    int timeRange = GSocket.GetBufToInt(ref Msg.Value);
                    sLoginType role = new sLoginType(true);
                    if (GSocket.GetBufToType<sLoginType>(ref Msg.Value, ref role))
                    {
                        // 啟動效能監測工具
                        _monitorUnit.StartMonitor();
                        _monitorUnit.SetTag(role.Account);

                        Console.WriteLine($"{role.Account}發送 註冊禮包碼");
                        _socket.PushDWord(10);
                        _socket.PushString("sayhello2you");
                        _socket.SendMsg(1, 111);
                    }
                    break;
                case 3:
                    // 創角帳號
                    Thread.Sleep(2000);
                    Console.WriteLine("創角帳號(kind):" + kind);
                    var randomGenerator = new RandomStringGenerator();
                    _socket.PushString(randomGenerator.RandomCreateCharacterName());
                    _socket.PushByte(sex);
                    _socket.PushByte(armType);
                    _socket.SendMsg(1, 72);
                    break;
            }
        }
    }
}