using Newtonsoft.Json;
using Nococid_API.Enums;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nococid_API.Services
{
    public interface IHttpRequestService
    {
        TResult Send<TResult>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers);
        TResult Send<TResult, TData>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers, TData data);
        void Send<TData>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers, TData data);
    }

    public class HttpRequestService : IHttpRequestService
    {
        private readonly IErrorHandlerService _errorHandler;

        public HttpRequestService(IErrorHandlerService errorHandler)
        {
            _errorHandler = errorHandler;
        }

        private void Close(HttpClient http_client, HttpResponseMessage response, StringContent string_content, HttpRequestMessage request)
        {
            if (response != null) response.Dispose();
            if (request != null) request.Dispose();
            if (string_content != null) string_content.Dispose();
            http_client.Dispose();
        }

        private TResult GetResult<TResult>(Uri uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers, string body)
        {
            HttpClient http_client = new HttpClient();
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            StringContent string_content = null;
            string content = null;

            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    string_content = new StringContent(body);
                    string_content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                foreach (var header in headers)
                {
                    http_client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                switch (method)
                {
                    case HttpRequestMethod.Get:
                        response = http_client.GetAsync(uri).GetAwaiter().GetResult();
                        break;
                    case HttpRequestMethod.Post:
                        response = http_client.PostAsync(uri, string_content).GetAwaiter().GetResult();
                        break;
                    case HttpRequestMethod.Put:
                        response = http_client.PutAsync(uri, string_content).GetAwaiter().GetResult();
                        break;
                    case HttpRequestMethod.Patch:
                        response = http_client.PatchAsync(uri, string_content).GetAwaiter().GetResult();
                        break;
                    case HttpRequestMethod.Delete:
                        if (!string.IsNullOrEmpty(body))
                        {
                            request = new HttpRequestMessage(new HttpMethod("DELETE"), uri)
                            {
                                Content = string_content
                            };
                            response = http_client.SendAsync(request).GetAwaiter().GetResult();
                        } else
                        {
                            response = http_client.DeleteAsync(uri).GetAwaiter().GetResult();
                        }
                        break;
                }
                try
                {
                    content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<TResult>(content);
                    }

                    throw _errorHandler.WriteLog("Request not success! Response with fail status code " + response.StatusCode + "! Url: " + uri.AbsoluteUri,
                        new Exception(content), DateTime.Now, "Server", "Service_HttpRequest");
                }
                catch (Exception e)
                {
                    throw e is ServerException ? e :_errorHandler.WriteLog("Could not read the response body!" + "! Url: " + uri.AbsoluteUri,
                        new Exception(e.Message + "\n" + content), DateTime.Now, "Server", "Service_HttpRequest");
                }
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("Service Error",
                        e, DateTime.Now, "Server", "Service_HttpRequest");
            }
            finally
            {
                Close(http_client, response, string_content, request);
            }
        }

        public TResult Send<TResult>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers)
        {
            return GetResult<TResult>(new Uri(uri), method, headers, "");
        }

        public TResult Send<TResult, TData>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers, TData data)
        {
            return GetResult<TResult>(new Uri(uri), method, headers, JsonConvert.SerializeObject(data));
        }

        public void Send<TData>(string uri, HttpRequestMethod method, KeyValuePair<string, string>[] headers, TData data)
        {
            GetResult<object>(new Uri(uri), method, headers, JsonConvert.SerializeObject(data));
        }
    }
}
