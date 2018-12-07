using GSMediator.StressTest.Model;
using GSMediator.StressTest.PerformaceMonitor;
using GSMediator.StressTest.Services;
using Newtonsoft.Json;
using System;
using System.Net;

namespace GSMediator.StressTest.Net.Service.MyTask
{
    public class RedemptionActivateTask
    {
        private readonly IRedemptionActivationService _redemptionActivationService;
        private readonly IMonitorManager _monitorManager;
        private readonly MonitorUnit _monitor;

        public RedemptionActivateTask(IRedemptionActivationService service,
            IMonitorManager monitorManager)
        {
            _redemptionActivationService = service;
            _monitorManager = monitorManager;
            _monitor = new MonitorUnit(_monitorManager);
        }

        public void RunTask(int index)
        {
            // 產生假的禮包碼兌換資料
            var token = string.Empty;
            var redemptionActivate = new RedemptionActivate()
            {
                UID = index.ToString(),
                ServerID = "1",
                PlatformID = 1,
                PackageName = "com.wmgame.tstd5.lenovo",
                RedemptionCode = "tstd5",
                SocketWho = 0
            };
            _monitor.SetStartPoint();
            _monitor.StartMonitor();
            _redemptionActivationService.CreateRedemptionActivationInfo(redemptionActivate, token, OnActivateResult);
        }

        /// <summary>
        /// Called when [activate result].
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="responseText">The response text.</param>
        private void OnActivateResult(HttpStatusCode httpStatusCode, string responseText)
        {
            if (httpStatusCode == HttpStatusCode.OK)
            {
                _monitor.StopAndSaveToMonitorManager(true, "OK");
            }
            else if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                var responseMsg = JsonConvert.DeserializeObject<ResponseMessage>(responseText);
                _monitor.StopAndSaveToMonitorManager(false, $"{responseMsg.Message}");
            }
            else
            {
                _monitor.StopAndSaveToMonitorManager(false, $"其他HTTP問題: {httpStatusCode}");
            }
            Console.WriteLine("Finish One Task.");
        }
    }
}