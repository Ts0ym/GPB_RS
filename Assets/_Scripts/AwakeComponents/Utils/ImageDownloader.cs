using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AwakeComponents.Utils
{
    public static class ImageDownloader
    {
        public static IEnumerator DownloadImage(string imageUrl, Action<Texture2D> onSuccess, Action<string> onError)
        {
            using (var www = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    onSuccess?.Invoke(texture);
                }
                else
                {
                    onError?.Invoke(www.error);
                }
            }
        }
    }
}