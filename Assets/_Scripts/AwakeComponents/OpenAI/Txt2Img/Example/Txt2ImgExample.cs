using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AwakeComponents.Utils;

namespace AwakeComponents.OpenAI
{
    public class Txt2ImgExample : MonoBehaviour
    {
        [SerializeField] private string apiKey;
        [SerializeField] private InputField promptInput;
        [SerializeField] private Dropdown sizeDropdown;
        [SerializeField] private Button submitButton;
        [SerializeField] private RawImage resultImage;
        [SerializeField] private InputField resultText;
        [SerializeField] private float imageScaleMultiplier = 0.2f;

        private void Start()
        {
            submitButton.onClick.AddListener(OnSubmit);
            sizeDropdown.options.Clear();
            foreach (var size in Enum.GetNames(typeof(Txt2Img.ImageSize)))
            {
                sizeDropdown.options.Add(new Dropdown.OptionData(size));
            }
            sizeDropdown.value = 0; // Set default value
        }

        private async void OnSubmit()
        {
            string prompt = promptInput.text;
            Txt2Img.ImageSize size = (Txt2Img.ImageSize)sizeDropdown.value;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(prompt))
            {
                resultText.text = "Please fill in all fields.";
                return;
            }

            submitButton.interactable = false; // Блокируем кнопку
            
            Txt2Img txt2Img = new Txt2Img(apiKey);

            await txt2Img.GenerateImage(
                prompt,
                size,
                imageUrl =>
                {
                    resultText.text = $"Success: Image generated from {imageUrl}";
                    StartCoroutine(ImageDownloader.DownloadImage(imageUrl, OnImageDownloaded, OnImageDownloadError));
                },
                error =>
                {
                    resultText.text = $"Error: {error}";
                    submitButton.interactable = true; // Разблокируем кнопку
                }
            );
        }

        private void OnImageDownloaded(Texture2D texture)
        {
            resultImage.texture = texture;
            resultImage.SetNativeSize();
            
            // Уменьшаем размер изображения
            resultImage.rectTransform.sizeDelta *= imageScaleMultiplier;

            resultText.text += "\nImage successfully downloaded.";
            submitButton.interactable = true; // Разблокируем кнопку
        }

        private void OnImageDownloadError(string error)
        {
            resultText.text = $"Error downloading image: {error}";
            submitButton.interactable = true; // Разблокируем кнопку
        }
    }
}
