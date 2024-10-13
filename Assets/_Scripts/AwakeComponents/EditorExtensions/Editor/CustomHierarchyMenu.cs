using UnityEngine;
using UnityEditor;

namespace AwakeComponents.EditorExtensions
{
    public class CustomHierarchyMenu
    {
        [MenuItem("GameObject/▲⚫ Awake!/SoftUI AMP/⛶ Fill parent", false, 10)]
        static void CreateSoftUIObjectFillParent()
        {
            CustomHierarchyMenuConfig config = CustomHierarchyMenuConfig.GetOrCreateConfig();

            if (config.softUIPrefabFillParent != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(config.softUIPrefabFillParent) as GameObject;

                if (Selection.activeGameObject == null) return;
                if (go == null) return;
                
                go.transform.SetParent(Selection.activeGameObject.transform, false);

                Undo.RegisterCreatedObjectUndo(go, "Create SoftUI AMP");
                Selection.activeGameObject = go;

                EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(go.transform.parent); };
            }
            else
            {
                Debug.LogError("SoftUI Prefab is not set in CustomHierarchyMenuConfig");
            }
        }

        [MenuItem("GameObject/▲⚫ Awake!/SoftUI AMP/⚫ At point", false, 11)]
        static void CreateSoftUIObjectAtPoint()
        {
            CustomHierarchyMenuConfig config = CustomHierarchyMenuConfig.GetOrCreateConfig();

            if (config.softUIPrefabAtPoint != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(config.softUIPrefabAtPoint) as GameObject;

                if (Selection.activeGameObject == null) return;
                if (go == null) return;
                
                go.transform.SetParent(Selection.activeGameObject.transform, false);

                Undo.RegisterCreatedObjectUndo(go, "Create SoftUI AMP");
                Selection.activeGameObject = go;

                EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(go.transform.parent); };
            }
            else
            {
                Debug.LogError("SoftUI Prefab is not set in CustomHierarchyMenuConfig");
            }
        }

        [MenuItem("GameObject/▲⚫ Awake!/Navigable UI Set", false, 12)]
        static void CreateNavigableUIObject()
        {
            CustomHierarchyMenuConfig config = CustomHierarchyMenuConfig.GetOrCreateConfig();

            if (config.navigableUISet != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(config.navigableUISet) as GameObject;

                if (Selection.activeGameObject == null) return;
                if (go == null) return;
                
                go.transform.SetParent(Selection.activeGameObject.transform, false);

                Undo.RegisterCreatedObjectUndo(go, "Create Navigable UI Set");
                Selection.activeGameObject = go;

                EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(go.transform.parent); };
            }
            else
            {
                Debug.LogError("Navigable UI Set Prefab is not set in CustomHierarchyMenuConfig");
            }
        }
        
        [MenuItem("GameObject/▲⚫ Awake!/SoftUI Canvas", false, 13)]
        static void CreateSoftUICanvas()
        {
            CustomHierarchyMenuConfig config = CustomHierarchyMenuConfig.GetOrCreateConfig();

            if (config.softUICanvas != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(config.softUICanvas) as GameObject;

                if (Selection.activeGameObject == null) return;
                if (go == null) return;
                
                go.transform.SetParent(Selection.activeGameObject.transform, false);

                Undo.RegisterCreatedObjectUndo(go, "Create SoftUI Canvas");
                Selection.activeGameObject = go;

                EditorApplication.delayCall += () => { EditorGUIUtility.PingObject(go.transform.parent); };
            }
            else
            {
                Debug.LogError("SoftUI Canvas Prefab is not set in CustomHierarchyMenuConfig");
            }
        }

        [MenuItem("GameObject/▲⚫ Awake!/Get info", false, 100)]
        static void GetInfo()
        {
            EditorUtility.DisplayDialog("Info", "Info will be here :-)", "Close");
        }
    }
}