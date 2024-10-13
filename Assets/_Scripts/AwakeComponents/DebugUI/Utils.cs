using System.Reflection;
using AwakeComponents.Statistics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwakeComponents.DebugUI
{
    public static class Utils
    {
        public static void DrawDefaultButtons()
        {
            GUILayout.BeginHorizontal();
            
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("↩ Close"))
                DebugUIRenderer.visible = false;
            
            GUI.backgroundColor = new Color(1, 0.5f, 0);
            if (GUILayout.Button("↻ Restart"))
            {
                StatisticsManager.Store("debug_ui.restart_scene");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("✖ Quit"))
            {
                StatisticsManager.Store("debug_ui.quit");
                Application.Quit();
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
        }
        
        public static string GetHierarchyString(object component)
        {
            if (component is Component monoBehaviourComponent)
            {
                Transform transform = monoBehaviourComponent.transform;
                string hierarchy = transform.name;
                while (transform.parent != null)
                {
                    transform = transform.parent;
                    hierarchy = transform.name + " / " + hierarchy;
                }
                return hierarchy;
            }
            
            return "Not a MonoBehaviour";
        }

        public static string GetComponentInfoString(object component)
        {
            var componentInfo = component.GetType().GetCustomAttribute<ComponentInfoAttribute>();

            try
            {
                return component.GetType().Name + " (v" + componentInfo.Version + ", " +
                       componentInfo.LastModifiedDate.ToShortDateString() + ")";
            }
            catch
            {
                return "ERROR GETTING INFO";
            }
        }
    }
}