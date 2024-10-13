using UnityEngine;
using UnityEditor;

namespace AwakeComponents.EditorExtensions
{
    [CustomEditor(typeof(CustomHierarchyMenuConfig))]
    public class CustomHierarchyMenuConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CustomHierarchyMenuConfig config = (CustomHierarchyMenuConfig)target;

            // Вывод пользовательского интерфейса для настройки префаба
            config.softUIPrefabAtPoint = (GameObject) EditorGUILayout.ObjectField(
                "Soft UI Prefab At Point", 
                config.softUIPrefabAtPoint, 
                typeof(GameObject), 
                false);

            config.softUIPrefabFillParent = (GameObject) EditorGUILayout.ObjectField(
                "Soft UI Prefab Fill Parent", 
                config.softUIPrefabFillParent, 
                typeof(GameObject), 
                false);
            
            config.navigableUISet = (GameObject) EditorGUILayout.ObjectField(
                "Navigable UI Set", 
                config.navigableUISet, 
                typeof(GameObject), 
                false);
            
            config.softUICanvas = (GameObject) EditorGUILayout.ObjectField(
                "Soft UI Canvas", 
                config.softUICanvas, 
                typeof(GameObject), 
                false);
        }
    }

}