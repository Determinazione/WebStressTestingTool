using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSMediator.Common.WebRequest
{
    class WebRequest : IWebRequest
    {
        private HttpClient _httpClient;
        public WebRequest(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The token.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public async Task<T> GetData<T>(string token, string uri)
        {
            SetToken(token);
            var responseString = await _httpClient.GetStringAsync(uri);
            var response = JsonConvert.DeserializeObject<T>(responseString);
            return response;
        }

        /// <summary>
        /// Gets the list data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The token.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public async Task<List<T>> GetListData<T>(string token, string uri)
        {
            SetToken(token);
            var responseString = await _httpClient.GetStringAsync(uri);
            var responseList = JsonConvert.DeserializeObject<List<T>>(responseString);
            return responseList;
        }

        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public async Task PostAsync(string uri, string jsonString, string token)
        {
            await PostAsync(uri, jsonString, token, (status, responseText) => { });
        }

        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public async Task PostAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback)
        {
            SetToken(token);
            var createContent = CreateContent(jsonString);
            // 發動Post
            var response = await _httpClient.PostAsync(uri, createContent);
            ProcessResponseMessage(response, callback);
        }

        /// <summary>
        /// Puts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public async Task PutAsync(string uri, string jsonString, string token)
        {
            await PutAsync(uri, jsonString, token, (status, responseText) => { });
        }

        /// <summary>
        /// Puts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public async Task PutAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback)
        {
            SetToken(token);
            var createContent = CreateContent(jsonString);
            // 發動Put
            var response = await _httpClient.PutAsync(uri, createContent);
            ProcessResponseMessage(response, callback);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public async Task DeleteAsync(string uri, string token)
        {
            await DeleteAsync(uri, token, (status, responseText) => { });
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public async Task DeleteAsync(string uri, string token, Action<HttpStatusCode, string> callback)
        {
            SetToken(token);
            // 發動Delete
            var response = await _httpClient.DeleteAsync(uri);
            ProcessResponseMessage(response, callback);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task DeleteAsync(string uri, string jsonString, string token)
        {
            await DeleteAsync(uri, jsonString, token, (status, responseText) => { });
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="token">The token.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public async Task DeleteAsync(string uri, string jsonString, string token, Action<HttpStatusCode, string> callback)
        {
            SetToken(token);
            var createContent = CreateContent(jsonString);
            // 發動Delete
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            request.Content = createContent;
            var response = await _httpClient.SendAsync(request);
            ProcessResponseMessage(response, callback);
        }

        /// <summary>
        /// Sets the token.
        /// </summary>
        /// <param name="token">The token.</param>
        private void SetToken(string token)
        {
            // 設定JWT驗證的Token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        private StringContent CreateContent(string jsonString)
        {
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// 確認Response是成功的，否則發出Exception
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="callback">The callback.</param>
        private async void ProcessResponseMessage(HttpResponseMessage response, Action<HttpStatusCode, string> callback)
        {
            try
            {
                // 檢測Web API已提供的Status Code，並進行相應處理
                string responseText = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.BadRequest ||
                    response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"{response.StatusCode} : {responseText}");
                    callback(response.StatusCode, $"{responseText}");
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HttpStatusCode:{response.StatusCode}");
                Console.WriteLine($"產生未處理的HttpRequest錯誤: {e.Message}");
            }
        }
    }
}
