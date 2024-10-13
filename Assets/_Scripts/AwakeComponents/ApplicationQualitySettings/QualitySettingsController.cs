using UnityEngine;
using AwakeComponents.DebugUI;

namespace AwakeComponents.ApplicationQualitySettings
{
    [ComponentInfo("1.0.0", "17.09.2024")]
    public class QualitySettingsController : MonoBehaviour, IDebuggableComponent
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool vSyncEnabled = true;
        [SerializeField] private int textureQuality = 0;
        [SerializeField] private int antiAliasingLevel = 0;
        [SerializeField] private int shadowQuality = 2;
        [SerializeField] private int anisotropicFiltering = 1;

        private const string PrefPrefix = "QualitySettings_";
        private const string FrameRatePrefKey = PrefPrefix + "TargetFrameRate";
        private const string VSyncPrefKey = PrefPrefix + "VSyncEnabled";
        private const string TextureQualityPrefKey = PrefPrefix + "TextureQuality";
        private const string AntiAliasingPrefKey = PrefPrefix + "AntiAliasing";
        private const string ShadowQualityPrefKey = PrefPrefix + "ShadowQuality";
        private const string AnisotropicFilteringPrefKey = PrefPrefix + "AnisotropicFiltering";

        void Start()
        {
            LoadSettings();
            ApplySettings();
        }

        void LoadSettings()
        {
            targetFrameRate = PlayerPrefs.GetInt(FrameRatePrefKey, targetFrameRate);
            vSyncEnabled = PlayerPrefs.GetInt(VSyncPrefKey, vSyncEnabled ? 1 : 0) == 1;
            textureQuality = PlayerPrefs.GetInt(TextureQualityPrefKey, textureQuality);
            antiAliasingLevel = PlayerPrefs.GetInt(AntiAliasingPrefKey, antiAliasingLevel);
            shadowQuality = PlayerPrefs.GetInt(ShadowQualityPrefKey, shadowQuality);
            anisotropicFiltering = PlayerPrefs.GetInt(AnisotropicFilteringPrefKey, anisotropicFiltering);
        }

        void ApplySettings()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
            QualitySettings.globalTextureMipmapLimit = textureQuality;
            QualitySettings.antiAliasing = antiAliasingLevel;
            QualitySettings.shadows = (ShadowQuality)shadowQuality;
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)anisotropicFiltering;
        }

        public void RenderDebugUI()
        {
            RenderSetting("Target Frame Rate", ref targetFrameRate, 10, 120, FrameRatePrefKey);
            RenderSetting("VSync", ref vSyncEnabled, VSyncPrefKey);
            RenderSetting("Texture Quality (0 = Full, 1 = Half, 2 = Quarter, 3 = Eighth)", ref textureQuality, 0, 3, TextureQualityPrefKey);
            RenderSetting("Anti-Aliasing (0 = Off, 2 = 2x, 4 = 4x, 8 = 8x)", ref antiAliasingLevel, 0, 8, AntiAliasingPrefKey);
            RenderSetting("Shadow Quality (0 = No Shadows, 1 = Hard Shadows, 2 = All Shadows)", ref shadowQuality, 0, 2, ShadowQualityPrefKey);
            RenderSetting("Anisotropic Filtering", ref anisotropicFiltering, 0, 1, AnisotropicFilteringPrefKey);
        }

        // Универсальная функция для рендеринга как слайдеров, так и переключателей
        void RenderSetting(string label, ref int value, int minValue, int maxValue, string prefKey)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}: ");
            int newValue = (int)GUILayout.HorizontalSlider(value, minValue, maxValue);
            GUILayout.Label(newValue.ToString());
            GUILayout.EndHorizontal();

            if (newValue != value)
            {
                value = newValue;
                PlayerPrefs.SetInt(prefKey, value);
                ApplySettings();
            }
        }

        // Перегруженная версия для bool (когда используется как 0 и 1)
        void RenderSetting(string label, ref bool value, string prefKey)
        {
            int boolValue = value ? 1 : 0;
            RenderSetting(label, ref boolValue, 0, 1, prefKey);
            value = boolValue == 1;
        }
    }
}
