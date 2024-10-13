using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AwakeComponents.ArtNet
{
    public class LedRainbowExample : MonoBehaviour
    {
        public LedController ledController;

        void Start()
        {
            if (ledController == null)
            {
                Debug.LogError("LedController не установлен!");
                return;
            }

            StartCoroutine(ShowRainbow());
        }

        public IEnumerator ShowRainbow()
        {
            yield return null;

            Color32[] rainbowColors = new Color32[]
            {
                new Color32(255, 0, 0, 255),    // Красный
                new Color32(255, 127, 0, 255),  // Оранжевый
                new Color32(255, 255, 0, 255),  // Желтый
                new Color32(0, 255, 0, 255),    // Зеленый
                new Color32(0, 0, 255, 255),    // Синий
                new Color32(75, 0, 130, 255),   // Индиго
                new Color32(148, 0, 211, 255)   // Фиолетовый
            };

            for (int i = 0; i < 7; i++)
            {
                Color32 color = rainbowColors[i];

                for (int j = 0; j < 35; j++)
                {
                    float brightness = j / 34f;

                    byte r = (byte)(color.r * brightness);
                    byte g = (byte)(color.g * brightness);
                    byte b = (byte)(color.b * brightness);

                    ledController.SetPixel(i * 35 + j, r, g, b);
                }
            }

            ledController.ShowStrip();
        }
    }
}
