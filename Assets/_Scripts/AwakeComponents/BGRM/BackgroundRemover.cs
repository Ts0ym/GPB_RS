using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AwakeComponents.BGRM
{
    public class BackgroundRemover
    {
        private readonly string apiKey;
        private static readonly string apiUrl = "https://api.carve.photos/api/v1/images/remove_bg";

        public BackgroundRemover(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task RemoveBackground(Texture2D image, Action<Texture2D> onSuccess, Action<string> onError)
        {
            byte[] imageData = image.EncodeToPNG();
            string base64Image = Convert.ToBase64String(imageData);

            var requestBody = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            requestBody.Add(imageContent, "image", "image.png");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

                try
                {
                    Debug.Log("Sending POST request to: " + apiUrl);
                    var response = await client.PostAsync(apiUrl, requestBody);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
                        string imageId = jsonResponse.image_id;
                        
                        await GetProcessedImage(imageId, onSuccess, onError);
                    }
                    else
                    {
                        var errorString = await response.Content.ReadAsStringAsync();
                        onError?.Invoke($"Error: {response.StatusCode}, Details: {errorString}");
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Exception: {ex.Message}");
                }
            }
        }

        private async Task GetProcessedImage(string imageId, Action<Texture2D> onSuccess, Action<string> onError)
        {
            string getResultUrl = $"https://api.carve.photos/api/v1/images/images/{imageId}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

                bool isCompleted = false;

                while (!isCompleted)
                {
                    try
                    {
                        Debug.Log("Sending GET request to: " + getResultUrl);
                        var response = await client.GetAsync(getResultUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            Debug.Log("Response String: " + responseString);
                            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);

                            if (jsonResponse.detail == "Image is processing")
                            {
                                Debug.Log("Image is still processing. Retrying...");
                                await Task.Delay(2000); // Wait for 2 seconds before retrying
                            }
                            else if (jsonResponse.ContainsKey("image_url"))
                            {
                                string imageUrl = jsonResponse.image_url;

                                if (string.IsNullOrEmpty(imageUrl))
                                {
                                    onError?.Invoke("Error: imageUrl is empty.");
                                    return;
                                }

                                Debug.Log("Sending GET request to download image from: " + imageUrl);
                                var imageResponse = await client.GetAsync(imageUrl);
                                if (imageResponse.IsSuccessStatusCode)
                                {
                                    byte[] imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                                    Texture2D texture = new Texture2D(2, 2);
                                    texture.LoadImage(imageBytes);
                                    onSuccess?.Invoke(texture);
                                }
                                else
                                {
                                    var errorString = await imageResponse.Content.ReadAsStringAsync();
                                    onError?.Invoke($"Error downloading image: {imageResponse.StatusCode}, Details: {errorString}");
                                }

                                isCompleted = true;
                            }
                            else
                            {
                                onError?.Invoke("Error: Unexpected response format.");
                                isCompleted = true;
                            }
                        }
                        else
                        {
                            var errorString = await response.Content.ReadAsStringAsync();
                            onError?.Invoke($"Error: {response.StatusCode}, Details: {errorString}");
                            isCompleted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke($"Exception: {ex.Message}");
                        isCompleted = true;
                    }
                }
            }
        }
    }
}
