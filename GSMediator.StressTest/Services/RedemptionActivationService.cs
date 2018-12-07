using GSMediator.Common.WebRequest;
using GSMediator.StressTest.Model;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GSMediator.StressTest.Services
{
    public class RedemptionActivationService : IRedemptionActivationService
    {
        // 都是指向同一個RedemptionCode API，但不同Container的URL
        private readonly string[] _remoteServiceBaseUrls = new string[] {
            "http://192.168.38.59:3001"
        };
        private readonly IWebRequest _webRequest;
        private readonly Random _random;

        public RedemptionActivationService(IWebRequest webRequest)
        {
            _webRequest = webRequest;
            _random = new Random();
        }

        /// <summary>
        /// Creates the redemption activation information.
        /// </summary>
        /// <param name="redemptionActivate">The redemption activate.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task CreateRedemptionActivationInfo(RedemptionActivate redemptionActivate, string token, Action<HttpStatusCode, string> callback)
        {
            // 使用隨機方式存取API，模擬負載均衡
            //var index = _random.Next(0, 2);
            var index = 0;
            var uri = $"{_remoteServiceBaseUrls[index]}/api/RedemptionActivation/RedemptionActivationInfo";
            await _webRequest.PostAsync(uri, JsonConvert.SerializeObject(redemptionActivate), token, callback);
        }
    }
}