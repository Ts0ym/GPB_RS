using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace AwakeCam
{
    public class Manager : MonoBehaviour
    {

        public static Manager instance;

        public GameObject camSet;

        public enum ConfigurationMode { NONE, POSITION, QUICK_CORNER, EDGE_BLENDING }
        public ConfigurationMode currentConfigurationMode = ConfigurationMode.NONE;

        public enum QuickCornerMode { NONE, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT };
        public QuickCornerMode currentQuickCornerMode = QuickCornerMode.NONE;

        public int currentConfiguringDisplay = -1;

        public int minDisplaysCount;

        public static List<CamSetController> camSets = new List<CamSetController>();

        public static bool debugModeEnabled;

        void Start()
        {
            if (Manager.instance != null)
                Destroy(this);
            else
                Manager.instance = this;

            for (int i = 0; i < Mathf.Max(Display.displays.Length, minDisplaysCount); i++)
            {
                if (i < Display.displays.Length)
                {
                    Display.displays[i].Activate();
                    Debug.Log("Display " + i + ": " + Display.displays[i].renderingWidth + "x" +
                              Display.displays[i].renderingHeight);
                }

                GameObject currentCamSet = Instantiate(camSet);
                currentCamSet.transform.parent = transform;
                currentCamSet.GetComponent<CamSetController>().targetDisplay = i;
                currentCamSet.GetComponent<CamSetController>().Init();

                camSets.Add(currentCamSet.GetComponent<CamSetController>());
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.C))
                debugModeEnabled = true;
        }

        private void OnGUI()
        {
            if (!debugModeEnabled)
                return;

            GUILayout.Label("Awake! displays configuration");
            GUILayout.Label("");

            if (currentConfiguringDisplay < 0)
            {
                GUILayout.Label("Select display to configure");

                for (int i = 0; i < Mathf.Max(Display.displays.Length, minDisplaysCount); i++)
                {
                    if (GUILayout.Button("[■] Display " + (i + 1)))
                        currentConfiguringDisplay = i;
                }
            }
            else
            {
                GUILayout.Label("Configurating display " + currentConfiguringDisplay);

                if (currentConfigurationMode == ConfigurationMode.NONE)
                {
                    if (GUILayout.Button("[✥] Configure position"))
                        currentConfigurationMode = ConfigurationMode.POSITION;

                    if (GUILayout.Button("[▣] Configure keystone"))
                        currentConfigurationMode = ConfigurationMode.QUICK_CORNER;
                    
                    if (GUILayout.Button("[◄] Configure edge blending"))
                        currentConfigurationMode = ConfigurationMode.EDGE_BLENDING;
                }
                else
                    GUILayout.Label("► Mode: " + (currentConfigurationMode == ConfigurationMode.POSITION ? "position" : "keystone"));

                if (currentConfigurationMode == ConfigurationMode.QUICK_CORNER)
                {
                    if (currentQuickCornerMode == QuickCornerMode.TOP_LEFT)
                        GUILayout.Label("[↖] Top left corner");
                    if (currentQuickCornerMode == QuickCornerMode.TOP_RIGHT)
                        GUILayout.Label("[↗] Top right corner");
                    if (currentQuickCornerMode == QuickCornerMode.BOTTOM_LEFT)
                        GUILayout.Label("[↙] Bottom left corner");
                    if (currentQuickCornerMode == QuickCornerMode.BOTTOM_RIGHT)
                        GUILayout.Label("[↘] Bottom right corner");
                    if (currentQuickCornerMode == QuickCornerMode.NONE)
                        GUILayout.Label("[?] Select corner");

                    if (currentQuickCornerMode == QuickCornerMode.NONE)
                    {
                        if (GUILayout.Button("[↖] Top left"))
                            currentQuickCornerMode = QuickCornerMode.TOP_LEFT;
                        if (GUILayout.Button("[↗] Top right"))
                            currentQuickCornerMode = QuickCornerMode.TOP_RIGHT;
                        if (GUILayout.Button("[↙] Bottom left"))
                            currentQuickCornerMode = QuickCornerMode.BOTTOM_LEFT;
                        if (GUILayout.Button("[↘] Bottom right"))
                            currentQuickCornerMode = QuickCornerMode.BOTTOM_RIGHT;
                    }
                }
                
                if (currentConfigurationMode == ConfigurationMode.EDGE_BLENDING)
                {
                    GUILayout.Label("Edge blending");
                    
                    GUILayout.Label("Top: " + camSets[currentConfiguringDisplay].edgeBlending.Top + "px");
                    camSets[currentConfiguringDisplay].edgeBlending.Top = 
                        GUILayout.HorizontalSlider(
                            camSets[currentConfiguringDisplay].edgeBlending.Top, 
                            0,
                            Display.displays.Length > currentConfiguringDisplay ?
                                Display.displays[currentConfiguringDisplay].renderingHeight : 1920
                            );
                    
                    GUILayout.Label("Bottom: " + camSets[currentConfiguringDisplay].edgeBlending.Bottom + "px");
                    camSets[currentConfiguringDisplay].edgeBlending.Bottom = 
                        GUILayout.HorizontalSlider(
                            camSets[currentConfiguringDisplay].edgeBlending.Bottom, 
                            0, 
                            Display.displays.Length > currentConfiguringDisplay ?
                                Display.displays[currentConfiguringDisplay].renderingHeight : 1920
                        );
                    
                    GUILayout.Label("Left: " + camSets[currentConfiguringDisplay].edgeBlending.Left + "px");
                    camSets[currentConfiguringDisplay].edgeBlending.Left = 
                        GUILayout.HorizontalSlider(
                            camSets[currentConfiguringDisplay].edgeBlending.Left, 
                            0, 
                            Display.displays.Length > currentConfiguringDisplay ?
                                Display.displays[currentConfiguringDisplay].renderingWidth : 1080
                        );
                    
                    GUILayout.Label("Right: " + camSets[currentConfiguringDisplay].edgeBlending.Right + "px");
                    camSets[currentConfiguringDisplay].edgeBlending.Right = 
                        GUILayout.HorizontalSlider(
                            camSets[currentConfiguringDisplay].edgeBlending.Right, 
                            0, 
                            Display.displays.Length > currentConfiguringDisplay ?
                                Display.displays[currentConfiguringDisplay].renderingWidth : 1080
                        );
                }

                GUILayout.Label("");

                if (GUILayout.Button("[<] Back"))
                {
                    if (currentQuickCornerMode != QuickCornerMode.NONE)
                        currentQuickCornerMode = QuickCornerMode.NONE;
                    else if (currentConfigurationMode != ConfigurationMode.NONE)
                        currentConfigurationMode = ConfigurationMode.NONE;
                    else
                        currentConfiguringDisplay = -1;
                }
            }

            GUILayout.Label("");

            if (GUILayout.Button("[▼] Save"))
            {
                foreach (CamSetController camSetController in Object.FindObjectsOfType<CamSetController>())
                    camSetController.Save();
            }

            if (GUILayout.Button("[X] Exit"))
            {
                currentConfigurationMode = ConfigurationMode.NONE;
                currentConfiguringDisplay = -1;
                debugModeEnabled = false;
            }

            GUILayout.Label("");

            if (GUILayout.Button("[←] Revert"))
            {
                foreach (CamSetController camSetController in Object.FindObjectsOfType<CamSetController>())
                    camSetController.Load();
            }

            if (GUILayout.Button("[!!!] Clear"))
            {
                foreach (CamSetController camSetController in Object.FindObjectsOfType<CamSetController>())
                {
                    camSetController.Clear();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}