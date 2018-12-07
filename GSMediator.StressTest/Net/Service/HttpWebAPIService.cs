using GSMediator.StressTest.Net.Service.MyTask;
using GSMediator.StressTest.PerformaceMonitor;
using GSMediator.StressTest.Services;
using System.Threading.Tasks;

namespace GSMediator.StressTest.Net.Service
{
    public class HttpWebAPIService
    {
        private readonly IRedemptionActivationService _redemptionActivationService;
        private readonly IMonitorManager _monitorManager;
        private int _connectTimes = 0;

        public HttpWebAPIService(IRedemptionActivationService service,
            IMonitorManager monitorManager)
        {
            _redemptionActivationService = service;
            _monitorManager = monitorManager;
        }

        public void StartNewTask()
        {
            var redemptionActivateTask = new RedemptionActivateTask(_redemptionActivationService, _monitorManager);
            Task task = Task.Factory.StartNew(() =>
            {
                _connectTimes++;
                redemptionActivateTask.RunTask(_connectTimes);
            });
        }
    }
}