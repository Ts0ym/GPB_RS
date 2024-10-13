using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AwakeComponents.Statistics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwakeComponents.DebugUI
{
    [ComponentInfo("1.1.2", "06.04.2024")]
    public class DebugUI : MonoBehaviour, IDebuggableComponent
    {
        // This component provides a debug UI for all components that implement the IDebuggableComponent interface
        public static object selectedComponent;
        
        private float timer = 0.0f;
        private const float UpdateInterval = 1.0f;
        
        public static DebugUI instance;
        
        public string password = "apwd";
        public static bool accessAllowed = false;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
            
            instance = this;
            
            // Automatically register all objects that implement the IDebuggableComponent interface
            ComponentManager.UpdateComponentsList();
        }

        private void Update()
        {
            // Toggle debug UI with Tab key
            if (Input.GetKeyDown(KeyCode.Tab)) ToggleDebugUIVisibility();
        }
        
        public static void ToggleDebugUIVisibility()
        {
            DebugUIRenderer.visible = !DebugUIRenderer.visible;

            if (instance.password != "") accessAllowed = false;
        }

        void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= UpdateInterval)
            {
                ComponentManager.UpdateComponentsList();
                timer = 0.0f;
            }
        }

        void OnGUI()
        {
            DebugUIRenderer.Render();
        }

        public void RenderDebugUI()
        {
            GUILayout.Label("This component does not have a debug UI yet");
        }
    }
}
