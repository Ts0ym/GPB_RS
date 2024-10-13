using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using UnityEngine;

namespace AwakeComponents.MultiDisplayActivator
{
    [ComponentInfo("1.0.1", "05.04.2024")]
    public class MultiDisplayManager : MonoBehaviour, IDebuggableComponent
    {
        void Start()
        {
            for (int i = 0; i < Display.displays.Length; i++)
                Display.displays[i].Activate();
        }
        
        public void RenderDebugUI()
        {
            GUILayout.Label("Displays count: " + Display.displays.Length);
        }
    }
}