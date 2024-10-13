using UnityEditor;
using UnityEngine;

namespace AwakeComponents.EditorExtensions
{
    public class CustomHierarchyMenuConfig : ScriptableObject
    {
        public GameObject softUICanvas;
        
        public GameObject softUIPrefabAtPoint;
        public GameObject softUIPrefabFillParent;

        public GameObject navigableUISet;

        // Метод для получения текущей конфигурации
        public static CustomHierarchyMenuConfig GetOrCreateConfig()
        {
            string assetPath = "Assets/AwakeComponents/EditorExtensions/Editor/CustomHierarchyMenuConfig.asset";
            var config = AssetDatabase.LoadAssetAtPath<CustomHierarchyMenuConfig>(assetPath);
            if (config == null)
            {
                // Проверяем, существует ли директория "Editor", и создаем ее, если нет
                if (!System.IO.Directory.Exists(assetPath))
                {
                    System.IO.Directory.CreateDirectory(assetPath);
                    AssetDatabase.Refresh(); // Обновляем базу данных ассетов Unity
                }

                config = CreateInstance<CustomHierarchyMenuConfig>();
                AssetDatabase.CreateAsset(config, assetPath);
                AssetDatabase.SaveAssets();
            }
            return config;
        }
    }

}