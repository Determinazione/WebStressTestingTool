using System;
using System.Threading;
using System.Threading.Tasks;

namespace GSMediator.StressTest.Net.Service
{
    public abstract class NetworkService : INetworkService
    {
        protected readonly IServiceProvider _serviceProvider;
        private const int SLEEP_TIME = 1000;

        public NetworkService(IServiceProvider provider)
        {
            _serviceProvider = provider;
            Awake();
        }

        public void Start()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(SLEEP_TIME);
                    Update();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public abstract void Awake();
        public abstract void Update();
    }
}
