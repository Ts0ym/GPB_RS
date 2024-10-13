using System;
using UnityEngine;

namespace AwakeComponents.Utils
{
    public static class ImageTools
    {
        public static string Texture2DToBase64(Texture2D texture)
        {
            byte[] imageData = texture.EncodeToPNG();
            return Convert.ToBase64String(imageData);
        }

        public static Texture2D Base64ToTexture2D(string base64)
        {
            string cleanBase64 = base64.Replace("data:image/png;base64,", "");
            byte[] imageData = Convert.FromBase64String(cleanBase64);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(imageData);
            return texture;
        }
    }
}