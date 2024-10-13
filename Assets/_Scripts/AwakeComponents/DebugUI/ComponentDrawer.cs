using System.Collections.Generic;
using AwakeComponents.Statistics;
using UnityEngine;

namespace AwakeComponents.DebugUI
{
    public static class ComponentDrawer
    {
        // Draw the list of components
        public static void DrawComponentsList()
        {
            GUILayout.Box(Utils.GetComponentInfoString(DebugUI.instance), GUILayout.ExpandWidth(true));

            // Buttons to close, restart and quit the game
            Utils.DrawDefaultButtons();

            // Divider
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            if (ComponentManager._components.Count == 0)
            {
                GUILayout.Label("No components found");
                return;
            }
            
            GUILayout.Label("Components list:");
            
            foreach (var component in ComponentManager._components)
            {
                // If component is a group of components
                
                if (component is List<object> list)
                {
                    var groupComponent = list[0];
                    string label = "[" + list.Count + "] " + Utils.GetComponentInfoString(groupComponent);
                    
                    if (GUILayout.Button(label))
                    {
                        //StatisticsManager.Store("debug_ui.select_group." + groupComponent.GetType().Name);
                        DebugUI.selectedComponent = component;
                    }
                }
                else
                {
                    string label = Utils.GetComponentInfoString(component);

                    if (GUILayout.Button(label))
                    {
                        StatisticsManager.Store("debug_ui.select_component." + component.GetType().Name);
                        DebugUI.selectedComponent = component;
                    }
                }
            }
        }
        
        public static void DrawComponentsGroup(List<object> componentsGroup)
        {
            GUI.backgroundColor = Color.yellow;
            
            if (GUILayout.Button("↩ Back"))
                DebugUI.selectedComponent = null;
            
            GUI.backgroundColor = Color.white;

            foreach (var component in componentsGroup)
            {
                string label = Utils.GetComponentInfoString(component);
                string hierarchy = Utils.GetHierarchyString(component);

                TextAnchor tmp = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                
                if (GUILayout.Button(hierarchy))
                {
                    StatisticsManager.Store("debug_ui.select_component." + component.GetType().Name);
                    DebugUI.selectedComponent = component;
                }

                GUI.skin.button.alignment = tmp;
            }
        }

        // Draw the UI for a specific component
        public static void DrawComponentUI(object component)
        {
            GUILayout.Box(Utils.GetComponentInfoString(component), GUILayout.ExpandWidth(true));

            GUI.backgroundColor = Color.yellow;
            
            if (GUILayout.Button("↩ Back"))
                DebugUI.selectedComponent = null;
            
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
                
            component.GetType().GetMethod("RenderDebugUI")?.Invoke(component, null);
        }
    }
}