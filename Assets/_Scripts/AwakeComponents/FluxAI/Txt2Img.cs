using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AwakeComponents.FluxAI
{
    public class Txt2Img
    {
        private readonly string apiKey;
        private readonly string apiUrl;
        private static readonly string defaultApiUrl = "https://fal.run/fal-ai/flux-pro";

        public enum ImageSize
        {
            SquareHd,
            Square,
            Portrait4_3,
            Portrait16_9,
            Landscape4_3,
            Landscape16_9
        }

        private static string GetSizeString(ImageSize size)
        {
            switch (size)
            {
                case ImageSize.SquareHd:
                    return "square_hd";
                case ImageSize.Square:
                    return "square";
                case ImageSize.Portrait4_3:
                    return "portrait_4_3";
                case ImageSize.Portrait16_9:
                    return "portrait_16_9";
                case ImageSize.Landscape4_3:
                    return "landscape_4_3";
                case ImageSize.Landscape16_9:
                    return "landscape_16_9";
                default:
                    return "landscape_4_3";
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
                        prompt = prompt,
                        image_size = GetSizeString(size),
                        num_inference_steps = 28,
                        guidance_scale = 3.5,
                        num_images = 1,
                        safety_tolerance = "2"
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Authorization", $"Key {apiKey}");

                    var response = await client.PostAsync(apiUrl, content);

                    Debug.Log($"[Txt2Img] Response: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Log("[Txt2Img] Request successful.");

                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
                        string imageUrl = jsonResponse.images[0].url;

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
