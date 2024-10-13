using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using AwakeComponents.Log;
using AwakeComponents.Utils;
using UnityEngine;
using Newtonsoft.Json;

namespace AwakeComponents.OpenAI
{
    public class Img2Txt
    {
        private readonly string apiKey;
        private readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

        public Img2Txt(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public IEnumerator Describe(string imageUrl, string prompt, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log("[Img2Txt] Describing image. Input is a URL. Prompt: " + prompt);
            yield return DescribeInternal(new { url = imageUrl }, prompt, onSuccess, onError);
        }

        public IEnumerator Describe(Texture2D image, string prompt, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log("[Img2Txt] Describing image. Input is a Texture2D. Prompt: " + prompt);
            byte[] imageData = image.EncodeToJPG();
            Debug.Log("[Img2Txt] Encoded image to JPG. Size: " + imageData.Length);
            yield return Describe(imageData, prompt, onSuccess, onError);
        }

        public IEnumerator Describe(byte[] imageData, string prompt, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log("[Img2Txt] Describing image. Input is a byte array. Prompt: " + prompt);
            
            bool uploadSuccess = false;
            string uploadedUrl = null;

            yield return ImageUploader.UploadImage(imageData,
                (imageUrl) =>
                {
                    uploadSuccess = true;
                    uploadedUrl = imageUrl.url;
                },
                error => onError?.Invoke(error));

            if (uploadSuccess)
            {
                Debug.Log("[Img2Txt] Image uploaded successfully. URL: " + uploadedUrl);
                yield return DescribeInternal(new { url = uploadedUrl }, prompt, onSuccess, onError);
            } else
            {
                Debug.LogError("[Img2Txt] Image upload failed.");
            }
        }

        private IEnumerator DescribeInternal(object image, string prompt, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log("[Img2Txt] Describing image internally. Prompt: " + prompt);

            using (var client = new HttpClient())
            {
                var requestBody = new
                {
                    model = "gpt-4-turbo",
                    messages = new object[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = prompt },
                                new { type = "image_url", image_url = image }
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var responseTask = client.PostAsync(apiUrl, content);
                yield return new WaitUntil(() => responseTask.IsCompleted);
                
                Debug.Log("[Img2Txt] Response received.");

                if (responseTask.Result.IsSuccessStatusCode)
                {
                    Debug.Log("[Img2Txt] Request successful.");

                    var responseStringTask = responseTask.Result.Content.ReadAsStringAsync();
                    yield return new WaitUntil(() => responseStringTask.IsCompleted);

                    var responseString = responseStringTask.Result;
                    
                    Debug.Log("[Img2Txt] Response: " + responseString);
                    
                    onSuccess?.Invoke(responseString);
                }
                else
                {
                    var errorStringTask = responseTask.Result.Content.ReadAsStringAsync();
                    yield return new WaitUntil(() => errorStringTask.IsCompleted);

                    var errorString = errorStringTask.Result;
                    
                    Debug.LogError("[Img2Txt] Request failed. Error: " + responseTask.Result.ReasonPhrase + ", Details: " + errorString);
                    
                    onError?.Invoke($"Error: {responseTask.Result.ReasonPhrase}, Details: {errorString}");
                }
            }
        }
    }
}
