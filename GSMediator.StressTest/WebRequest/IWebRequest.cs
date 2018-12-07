using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GSMediator.Common.WebRequest
{
    public interface IWebRequest
    {
        Task DeleteAsync(string uri, string token);

        Task DeleteAsync(string uri, string token, Action<HttpStatusCode, string> callback);

        Task DeleteAsync(string uri, string jsonString, string token);

        Task DeleteAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback);

        Task<T> GetData<T>(string token, string uri);

        Task<List<T>> GetListData<T>(string token, string uri);

        Task PostAsync(string uri, string jsonString, string token);

        Task PostAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback);

        Task PutAsync(string uri, string jsonString, string token);

        Task PutAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback);
    }
}