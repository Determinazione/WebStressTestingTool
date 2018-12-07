using System;
using System.Net;
using System.Threading.Tasks;
using GSMediator.StressTest.Model;

namespace GSMediator.StressTest.Services
{
    public interface IRedemptionActivationService
    {
        Task CreateRedemptionActivationInfo(RedemptionActivate redemptionActivate, string token, Action<HttpStatusCode, string> callback);
    }
}