using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using AwakeComponents.DebugUI;

namespace AwakeComponents.ArtNet
{
    [ComponentInfo("1.0", "09.10.2024")]
    public class LedController : MonoBehaviour, IDebuggableComponent
    {
        public DmxController controller;

        public int numLeds = 0;
        byte[] leds;

        byte ledByRow;

        void Start()
        {
            leds = new byte[numLeds * 3];

            // Загрузка значения из PlayerPrefs или установка значения по умолчанию
            ledByRow = (byte)PlayerPrefs.GetInt("LedByRow", 35);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void ShowStrip()
        {
            // One universe handles only 512/3 = 170 LEDs
            int universes = numLeds / 170 + 1;
            byte[] universeData = new byte[512];

            // Last 2 bytes are 0 for all universes
            universeData[510] = 0;
            universeData[511] = 0;

            for (short i = 0; i < universes; i++)
            {
                // Calculate left array size
                int size = leds.Length - i * 510;

                // Copy the data
                System.Buffer.BlockCopy(leds, i * 510, universeData, 0, (size < 510) ? size : 510);

                // Fill the rest with zeros
                for (int j = size; j < 510; j++)
                {
                    universeData[j] = 0;
                }

                controller.Send(i, universeData);
            }
        }

        public void SetPixel(int Pixel, byte red, byte green, byte blue)
        {
            leds[Pixel * 3] = red;
            leds[Pixel * 3 + 1] = green;
            leds[Pixel * 3 + 2] = blue;
        }

        public void SetAll(byte red, byte green, byte blue)
        {
            for (int i = 0; i < numLeds; i++)
            {
                SetPixel(i, red, green, blue);
            }
        }

        public void RenderDebugUI()
        {
            // Ввод для изменения количества светодиодов в строке
            GUILayout.BeginHorizontal();
            GUILayout.Label("LEDs per Row:");
            string input = GUILayout.TextField(ledByRow.ToString(), 3);
            
            // Проверка на ввод корректного значения
            if (int.TryParse(input, out int result))
            {
                if (result > 0)
                {
                    ledByRow = (byte)result;

                    // Сохранение нового значения в PlayerPrefs
                    PlayerPrefs.SetInt("LedByRow", ledByRow);
                    PlayerPrefs.Save();
                }
            }

            GUILayout.EndHorizontal();

            // Рассчитываем количество светодиодов
            int numLeds = leds.Length / 3;

            // Начало вертикальной группы для размещения рядов светодиодов
            GUILayout.BeginVertical();

            for (int i = 0; i < numLeds; i++)
            {
                // Каждый раз, когда набирается нужное количество светодиодов, начинаем новый ряд
                if (i % ledByRow == 0)
                {
                    GUILayout.BeginHorizontal();
                }

                // Извлекаем значения RGB для каждого светодиода
                byte r = leds[i * 3];
                byte g = leds[i * 3 + 1];
                byte b = leds[i * 3 + 2];

                // Создаем цвет из значений RGB
                Color ledColor = new Color32(r, g, b, 255);

                // Задаем цвет для GUI
                GUI.color = ledColor;

                // Создаем более яркий квадрат для светодиода
                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.normal.background = Texture2D.whiteTexture;

                // Отображаем квадрат для текущего светодиода
                GUILayout.Box(GUIContent.none, style, GUILayout.Width(10), GUILayout.Height(10));

                // Завершаем горизонтальную группу после определенного количества светодиодов
                if ((i + 1) % ledByRow == 0)
                {
                    GUILayout.EndHorizontal();
                }
            }

            // Закрываем последнюю горизонтальную группу, если количество светодиодов не кратно `ledByRow`
            if (numLeds % ledByRow != 0)
            {
                GUILayout.EndHorizontal();
            }

            // Закрытие вертикальной группы
            GUILayout.EndVertical();
        }
    }
}
