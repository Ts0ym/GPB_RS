using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwakeComponents.WebRequests.Examples
{
    public class FaceSwapRequestExample: MonoBehaviour
    {
        [FormerlySerializedAs("_SourceImage")] public Image sourceImage;
        [FormerlySerializedAs("_targetImage")] public Image targetImage;
        
        [FormerlySerializedAs("_resultSwapFace")] public Image resultSwapFace;
        [FormerlySerializedAs("_resultSwapTexture")] public Texture2D resultSwapTexture;
        
        public HttpRequest httpRequest;
        
        [FormerlySerializedAs("URL")] public string url = "";
        // select request method SendRequestHttpClient or SendRequestString
        [FormerlySerializedAs("useHttpClient")] public bool useHttpClient = true;
        
        public void Detect()
        {
            var sourceTexture = sourceImage.sprite.texture;
            var targetTexture = targetImage.sprite.texture;
            
            url = "http://127.0.0.1:8090";
            
            Debug.Log("Sending to: " + url + "/swap-unity");
            
            // Send request use HttpRequest class
            // Var data dictionary strings
            var data = new Dictionary<string, string>
            {
                {"source", Utils.ImageTools.Texture2DToBase64(sourceTexture)},
                {"target", Utils.ImageTools.Texture2DToBase64(targetTexture)}
            };
            
            if (useHttpClient)
            {
                // Запрос отправляется с использованием метода SendRequestHttpClient асинхронно
                httpRequest.SendRequestHttpClient(url + "/swap-unity", HttpMethod.Post, data).ContinueWith(task_sw =>
                {
                    if (task_sw.Result.Success)
                    {
                        Debug.Log("Response: " + task_sw.Result.Response);
                    }
                    else
                    {
                        Debug.LogError("Error while creating thread: " + task_sw.Result.Response);
                    }
                });
            }
            else
            {
                // Запрос отправляется с использованием метода SendRequestString и корутины
                httpRequest.SendUnityWebRequest(url + "/swap-unity", HttpRequest.Method.POST, data);
            }
        }
        
        public void SetTargetImage(string base64)
        {
            resultSwapTexture = Utils.ImageTools.Base64ToTexture2D(base64);
            resultSwapFace.sprite = Sprite.Create(resultSwapTexture, new Rect(0, 0, resultSwapTexture.width, resultSwapTexture.height), new Vector2(0.5f, 0.5f));
        }

        private void SetTexture2D(string base64)
        {
            resultSwapTexture = Utils.ImageTools.Base64ToTexture2D(base64);
            resultSwapFace.sprite = Sprite.Create(resultSwapTexture, new Rect(0, 0, resultSwapTexture.width, resultSwapTexture.height), new Vector2(0.5f, 0.5f));
        }
        
        /*private async Task<(bool Success, string Response)> Swap(string url ,Data data)
        {
            var response = await SendRequestAsync(url, JsonConvert.SerializeObject(data), HttpMethod.Post);
            
            if (response == null)
            {
                return (false, "Error HTTP request");
            }
            
            return (true, response);
        }*/
        
        /*private async Task<string> SendRequestAsync(string url, string data, HttpMethod method, string scrf = "")
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, url)
                {
                    Content = data != null ? new StringContent(data, Encoding.UTF8, "application/json") : null
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
        
        [System.Serializable]
        public class Data
        {
            public string source;
            public string target;
        }

        [System.Serializable]
        public class ResponseData
        {
            public bool success;
            public string response;
        }*/
    }
}