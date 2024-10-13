using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using AwakeComponents.Utils;
using Newtonsoft.Json;

namespace AwakeComponents.OpenAI
{
    public class Img2TxtExample : MonoBehaviour
    {
        [SerializeField] private string apiKey;
        [SerializeField] private InputField urlInput;
        [SerializeField] private InputField promptInput;
        [SerializeField] private Button submitButton;
        [SerializeField] private InputField resultText;

        private void Start()
        {
            submitButton.onClick.AddListener(OnSubmit);
        }

        private void OnSubmit()
        {
            string prompt = promptInput.text;
            string url = urlInput.text;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(prompt))
            {
                resultText.text = "Please fill in all fields.";
                return;
            }

            submitButton.interactable = false; // Блокируем кнопку

            if (string.IsNullOrEmpty(url))
            {
                StartCoroutine(UploadAndDescribeImage(prompt));
            }
            else
            {
                StartCoroutine(DescribeImage(url, prompt));
            }
        }

        private IEnumerator UploadAndDescribeImage(string prompt)
        {
            string imagePath = Path.Combine(Application.streamingAssetsPath, "test.jpg");

            Texture2D image = new Texture2D(2, 2);

            if (imagePath.Contains("://") || imagePath.Contains(":///"))
            {
                using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imagePath))
                {
                    yield return unityWebRequest.SendWebRequest();
                    if (unityWebRequest.result != UnityWebRequest.Result.Success)
                    {
                        resultText.text = $"Error loading image: {unityWebRequest.error}";
                        submitButton.interactable = true;
                        yield break;
                    }
                    image = DownloadHandlerTexture.GetContent(unityWebRequest);
                }
            }
            else
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                image.LoadImage(imageData);
            }

            Img2Txt img2Txt = new Img2Txt(apiKey);
            yield return img2Txt.Describe(image, prompt,
                response =>
                {
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);
                    resultText.text = jsonResponse.choices[0].message.content;
                    submitButton.interactable = true; // Разблокируем кнопку
                },
                error =>
                {
                    resultText.text = $"Error: {error}";
                    submitButton.interactable = true; // Разблокируем кнопку
                });
        }

        private IEnumerator DescribeImage(string url, string prompt)
        {
            Img2Txt img2Txt = new Img2Txt(apiKey);
            yield return img2Txt.Describe(url, prompt,
                response =>
                {
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);
                    resultText.text = jsonResponse.choices[0].message.content;
                    submitButton.interactable = true; // Разблокируем кнопку
                },
                error =>
                {
                    resultText.text = $"Error: {error}";
                    submitButton.interactable = true; // Разблокируем кнопку
                });
        }
    }
}
