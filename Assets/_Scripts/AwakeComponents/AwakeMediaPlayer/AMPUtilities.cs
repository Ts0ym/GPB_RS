using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AwakeComponents.AwakeMediaPlayer
{
    public static class AMPUtilities
    {
        // Создание RenderTexture на основе RectTransform
        public static RenderTexture CreateRenderTexture(RectTransform rectTransform)
        {
            int width = Mathf.Max(1, (int)rectTransform.rect.width);
            int height = Mathf.Max(1, (int)rectTransform.rect.height);

            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            renderTexture.antiAliasing = 1;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.wrapMode = TextureWrapMode.Clamp;

            return renderTexture;
        }

        // Асинхронная загрузка текстуры из файла
        public static IEnumerator LoadTextureAsync(string filePath, System.Action<Texture2D> onLoaded)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    onLoaded?.Invoke(texture);
                }
                else
                {
                    Debug.LogError("Error: " + uwr.error);
                    onLoaded?.Invoke(null);
                }
            }
        }
        
        public static void ShowVideoLoader(GameObject parent, Sprite loaderSprite)
        {
            // Instantiate image
            if (parent.transform.Find("AMP_Loader") != null)
            {
                return;
            }
            
            var loader = new GameObject("AMP_Loader", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            loader.transform.SetParent(parent.transform, false);
            
            // Set it in the middle and fill the parent
            var rectTransform = loader.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            rectTransform.localScale = Vector3.one * 0.5f;
            
            // Set image properties
            var image = loader.GetComponent<UnityEngine.UI.Image>();
            image.sprite = loaderSprite;
            
            // Set sprite preserve aspect
            image.preserveAspect = true;
            
            // Set sprite to fit inside the parent witout cropping and stretching
        }
        
        public static void HideVideoLoader(GameObject parent)
        {
            var loader = parent.transform.Find("AMP_Loader");

            if (loader != null)
            {
                Object.Destroy(loader.gameObject);
            }
        }
        
        // Другие утилитарные методы, которые могут понадобиться, можно добавить здесь
    }
}