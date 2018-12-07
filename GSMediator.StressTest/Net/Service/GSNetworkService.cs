using GSMediator.StressTest.PerformaceMonitor;
using GSMediator.StressTest.Generator;
using System;

namespace GSMediator.StressTest.Net.Service
{
    internal class GSNetworkService : NetworkService
    {
        private const int LOGIN_PLATORM_ANDROID = 1;
        private const int LOGIN_ACCOUNT_TYPE_CUSTOM = 0;
        private const int LOGIN_DIRECT = 1;

        private MonitorUnit _monitorUnit;
        private SocketHandler _socketHandler;
        private int ClientVersion = 9;

        public GSNetworkService(IServiceProvider provider) : base(provider)
        {
        }

        public override void Awake()
        {
            var monitorManager = (IMonitorManager)_serviceProvider.GetService(typeof(IMonitorManager));
            _monitorUnit = new MonitorUnit(monitorManager);
            _socketHandler = new SocketHandler(SocketConnectedCallback, _monitorUnit);
        }

        public override void Update()
        {
            _socketHandler.UpdateGSocket();
        }

        /// <summary>
        /// Sockets the connected callback.
        /// </summary>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        private void SocketConnectedCallback(bool connected)
        {
            Console.WriteLine("Socket連線狀態:" + connected);
            if (connected)
            {
                LoginMediatorAccount();
            }
        }

        private void LoginMediatorAccount()
        {
            // 設置監測工具的起跑位置
            _monitorUnit.SetStartPoint();

            _socketHandler.GSocket.PushDWord(ClientVersion);
            _socketHandler.GSocket.PushByte(LOGIN_PLATORM_ANDROID);
            _socketHandler.GSocket.PushByte(LOGIN_ACCOUNT_TYPE_CUSTOM);

            // 取出固定式帳號密碼進行登入或註冊
            var accountGenerator = (AccountGenerator)_serviceProvider.GetService(typeof(AccountGenerator));
            var account = accountGenerator.DequeueAccount();
            Console.WriteLine($"取出帳號: {account}");
            _socketHandler.GSocket.PushString(account);
            // 密碼也跟帳號一模一樣
            _socketHandler.GSocket.PushString(account);

            _socketHandler.GSocket.PushByte(LOGIN_DIRECT);
            _socketHandler.GSocket.SendMsg(1, 71);
        }
    }
}