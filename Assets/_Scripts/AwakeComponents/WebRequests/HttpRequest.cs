using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text; 
using AwakeComponents.DebugUI;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace AwakeComponents.WebRequests
{
    [ComponentInfo("0.0.3", "10.08.2024")]
    public class HttpRequest : MonoBehaviour, IDebuggableComponent
    {
        private string _url;
        private string _method;
        
        public UnityEvent<string> onResponse = new();
        public UnityEvent<string> onError = new();
        
        #region HttpClient
        // Send Request use Task and HttpClient
        public async Task<(bool Success, string Response)> SendRequestHttpClient(string url, HttpMethod method ,Dictionary<string, string> data = null, Dictionary<string, string> headers = null)
        {
            var response = await SendRequestAsync(url, JsonConvert.SerializeObject(data), method, headers);
            
            if (response == null)
            {
                onError?.Invoke(response);
                return (false, "Error HTTP request");
            }
            onResponse?.Invoke(response);
            
            return (true, response);
        }
        
        private async Task<string> SendRequestAsync(string url, string data, HttpMethod method, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                // Check if headers exist and add them to the request
                if (headers != null && headers.Count != 0)
                {
                    foreach (var (key, value) in headers)
                    {
                        client.DefaultRequestHeaders.Add(key, value);
                    }
                }
                
                var request = new HttpRequestMessage(method, url)
                {
                    Content = data != null ? new StringContent(data, Encoding.UTF8, "application/json") : null
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion
        
        #region UnityWebRequest
        // Send Request use Coroutine and UnityWebRequest
        public void SendUnityWebRequest(string url, Method method, Dictionary<string, string> data = null, Dictionary<string, string> headers = null)
        {
            StartCoroutine(SendRequest(url, method, data, headers, httpResponse =>
            {
                try
                {
                    onResponse?.Invoke(httpResponse);
                }
                catch (ArgumentException e)
                {
                    onError?.Invoke(httpResponse);
                }
            }));
        }

        static IEnumerator SendRequest(string url, Method method, Dictionary<string, string> data, Dictionary<string, string> headers, Action<string> onResponse)
        {
            WWWForm form = new WWWForm();

            // Check if headers exist and add them to the request
            if (headers != null && headers.Count != 0)
            {
                foreach (var (key, value) in headers)
                {
                    form.headers.Add(key, value);
                }
            }
            
            // Add data to the form
            if (data != null && data.Count != 0)
            {
                foreach (var (key, value) in data)
                {
                    form.AddField(key, value);
                }
            }

            UnityWebRequest uwr = new UnityWebRequest();
            // swith case for different methods
            switch (method)
            {
                case Method.GET:
                    uwr = UnityWebRequest.Get(url);
                    break;
                case Method.POST:
                    uwr = UnityWebRequest.Post(url, form);
                    break;
            }

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + uwr.downloadHandler.text);
                onResponse?.Invoke(uwr.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error While Sending: " + uwr.error);
                onResponse?.Invoke("Error While Sending: " + uwr.error);
            }
        }
        [System.Serializable]
        public enum Method
        {
            GET,
            POST,
        }
        #endregion
        
        
        #region DebugUi
        
        public void RenderDebugUI()
        {
            
        }
        
        #endregion
    }
}

