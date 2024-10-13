using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AwakeComponents.OpenAI
{
    public class Txt2Img
    {
        private readonly string apiKey;
        private readonly string apiUrl;
        private static readonly string defaultApiUrl = "https://api.openai.com/v1/images/generations";

        public enum ImageSize
        {
            Square,
            Vertical,
            Horizontal
        }

        private static string GetSizeString(ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Square:
                    return "1024x1024";
                case ImageSize.Vertical:
                    return "1024x1792";
                case ImageSize.Horizontal:
                    return "1792x1024";
                default:
                    return "1024x1024";
            }
        }

        public Txt2Img(string apiKey, string apiUrl = null)
        {
            this.apiKey = apiKey;
            this.apiUrl = apiUrl ?? defaultApiUrl;
        }

        public async Task GenerateImage(string prompt, ImageSize size, Action<string> onSuccess, Action<string> onError)
        {
            Debug.Log("[Txt2Img] Generating image. Prompt: " + prompt);
            
            try
            {
                using (var client = new HttpClient())
                {
                    var requestBody = new
                    {
                        model = "dall-e-3",
                        prompt = prompt,
                        size = GetSizeString(size)
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    var response = await client.PostAsync(apiUrl, content);
                    
                    Debug.Log($"[Txt2Img] Response: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Log("[Txt2Img] Request successful.");

                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
                        string imageUrl = jsonResponse.data[0].url;
                        
                        Debug.Log("[Txt2Img] Image generated successfully. URL: " + imageUrl);
                        
                        onSuccess?.Invoke(imageUrl);
                    }
                    else
                    {
                        var errorString = await response.Content.ReadAsStringAsync();
                        
                        Debug.LogError($"[Txt2Img] Error: {response.ReasonPhrase}, Details: {errorString}");
                        
                        onError?.Invoke($"Error: {response.ReasonPhrase}, Details: {errorString}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Txt2Img] Exception: {ex.Message}");
                onError?.Invoke($"Exception: {ex.Message}");
            }
        }
    }
}
