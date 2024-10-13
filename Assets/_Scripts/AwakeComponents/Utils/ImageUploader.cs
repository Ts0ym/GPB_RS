using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AwakeComponents.Utils
{
    public static class ImageUploader
    {
        //private static readonly string uploadUrl = "http://77.221.133.56/image-vault/store.php";
        private static readonly string uploadUrl = "http://john-stranger.online/upload-file";

        public static IEnumerator UploadImage(byte[] imageData, Action<dynamic> onSuccess, Action<string> onError, string qrColor = null, int qrSize = 0)
        {
            Debug.Log("[ImageUploader] Uploading image...");
            
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", imageData, "image.png", "image/png");
            
            if (qrColor != null)
                form.AddField("color", qrColor);
            if (qrSize != 0)
                form.AddField("size", qrSize);

            using (UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form))
            {
                yield return www.SendWebRequest();

                Debug.Log("[ImageUploader] Upload response: " + www.downloadHandler.text);
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("[ImageUploader] Upload successful.");

                    string response = www.downloadHandler.text;
                    //var jsonResponse = JsonUtility.FromJson<UploadResponse>(response);
                    dynamic jsonResponse = JsonConvert.DeserializeObject(response);
                    
                    if (jsonResponse.url != null)
                    {
                        Debug.Log("[ImageUploader] Image uploaded successfully. URL: " + jsonResponse.url);
                        onSuccess?.Invoke(jsonResponse);
                    }
                    else
                    {
                        Debug.LogError("[ImageUploader] Upload failed: " + jsonResponse.error);
                        onError?.Invoke(jsonResponse.error);
                    }
                }
                else
                {
                    Debug.LogError("[ImageUploader] Upload failed: " + www.error);
                    onError?.Invoke(www.error);
                }
            }
        }

        [Serializable]
        private class UploadResponse
        {
            public string url;
            public string error;
            public string qr;
        }
    }
}