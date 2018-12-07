using GSMediator.Common.WebRequest;
using GSMediator.StressTest.Generator;
using GSMediator.StressTest.Net.Service;
using GSMediator.StressTest.PerformaceMonitor;
using GSMediator.StressTest.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace GSMediator.StressTest
{
    class Program
    {
        public static int MultiTaskTimes = 5000;

        static void Main(string[] args)
        {
            Console.WriteLine("===GS Mediator Stress Test===");

            var services = new ServiceCollection()
                .AddCustomServices()
                .AddCustomHttpClient();

            var provider = services.BuildServiceProvider();

            // 啟用GS服務 (高併發1000次)
            for (int i = 0; i < MultiTaskTimes; i++)
            {
                //provider.GetService<INetworkService>().Start();
                provider.GetService<HttpWebAPIService>().StartNewTask();
            }

            Console.ReadLine();
            // 輸出壓力測試報告
            provider.GetService<IMonitorManager>().OutputReport();
            Console.ReadLine();
        }
    }

    internal static class CustomExtensionsMethods
    {
        /// <summary>
        /// 注入需要的Service
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // 監控壓力測試結果的類別
            services.AddSingleton<IMonitorManager, MonitorManager>();
            // 測試Web API用的Service
            services.AddSingleton<HttpWebAPIService>();
            // 固定式帳號密碼產生器
            services.AddSingleton<AccountGenerator>();
            services.AddTransient<IRedemptionActivationService, RedemptionActivationService>();
            services.AddTransient<INetworkService, GSNetworkService>();
            return services;
        }


        /// <summary>
        /// 注入HttpClientService
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<WebRequest>();
            services.AddTransient<IWebRequest, WebRequest>();
            return services;
        }
    }
}
