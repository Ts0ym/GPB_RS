using System.Collections.Generic;
using System.Reflection;
using AwakeComponents.Statistics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwakeComponents.DebugUI
{
    public class DebugUIRenderer
    {
        public static bool visible;
        
        private static Vector2 _scrollPosition;

        public static void Render()
        {
            if (visible)
            {
                if (!DebugUI.accessAllowed && DebugUI.instance.password != "")
                    ShowPasswordInput();
                else
                    DrawDebugUI();
            }
            else
                DrawActivationButton();
        }
        
        static string passwordToCheck = "";
        private static void ShowPasswordInput()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Enter password to access debug UI:");
            
            passwordToCheck = GUILayout.PasswordField(passwordToCheck, '*');

            if (GUILayout.Button("OK"))
            {
                if (passwordToCheck == DebugUI.instance.password)
                    DebugUI.accessAllowed = true;
                else
                    DebugUI.ToggleDebugUIVisibility();
                
                passwordToCheck = "";
            }

            GUILayout.EndVertical();
        }
        
        private static void DrawDebugUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical(GUI.skin.box);
            
            SetGuiStyles();

            ShowGlobalErrors();
            
            if (DebugUI.selectedComponent == null) 
                ComponentDrawer.DrawComponentsList();
            else if (DebugUI.selectedComponent is List<object> list)
                ComponentDrawer.DrawComponentsGroup(list);
            else
                ComponentDrawer.DrawComponentUI(DebugUI.selectedComponent);
            
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private static void SetupTransparentButtonStyle(GUIStyle style)
        {
            style.normal.background = null;
            style.hover.background = null;
            style.active.background = null;
            style.border = new RectOffset(0, 0, 0, 0);
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);
            style.overflow = new RectOffset(0, 0, 0, 0);
        }

        private static void DrawActivationButton()
        {
            GUIStyle transparentButtonStyle = new GUIStyle();
            SetupTransparentButtonStyle(transparentButtonStyle);

            if (GUI.Button(new Rect(0, 0, 100, 100), "", transparentButtonStyle))
                visible = !visible;
        }

        private static void SetGuiStyles()
        {
            GUI.skin.label.fontSize = 12;
            GUI.skin.button.fontSize = 12;
            GUI.skin.box.fontSize = 12;
            
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            
            GUI.skin.label.wordWrap = true;
            GUI.skin.button.wordWrap = true;
            GUI.skin.box.wordWrap = true;
            
            GUI.skin.label.normal.textColor = Color.white;
            GUI.skin.button.normal.textColor = Color.white;
            GUI.skin.box.normal.textColor = Color.white;
            
            GUI.backgroundColor = Color.white;
            
            GUI.skin.label.padding = new RectOffset(5, 5, 5, 5);
            GUI.skin.button.padding = new RectOffset(5, 5, 5, 5);
            GUI.skin.box.padding = new RectOffset(5, 5, 5, 5);
            
            GUI.skin.label.margin = new RectOffset(3, 3, 3, 3);
            GUI.skin.button.margin = new RectOffset(3, 3, 3, 3);
            GUI.skin.box.margin = new RectOffset(3, 3, 3, 3);

            GUI.skin.label.stretchWidth = true;
            GUI.skin.button.stretchWidth = true;
            GUI.skin.box.stretchWidth = true;

            // Remove gradient background
            GUI.skin.label.normal.background = null;
            GUI.skin.button.normal.background = null;
            GUI.skin.box.normal.background = null;
        }
        
        private static void ShowGlobalErrors() // TODO: Refactor this method
        {
#if UNITY_EDITOR
            GUI.backgroundColor = Color.red;

            if (PlayerSettings.companyName != "Awake!")
            {
                GUI.Box(new Rect(10, 10, 200, 100), "Please set the company name in Player Settings:\n\nAwake!");
            }
            else if (StatisticsManager.instance == null)
            {
                GUI.Box(new Rect(10, 10, 200, 100), "Please add the StatisticsManager component to the scene!");
            }
            
            GUI.backgroundColor = Color.white;
#endif
        }
    }

}