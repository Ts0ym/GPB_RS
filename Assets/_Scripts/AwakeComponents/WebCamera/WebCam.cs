using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeComponents.WebCamera
{
    [ComponentInfo("0.1", "06.08.2024")]
    public class WebCam : MonoBehaviour, IDebuggableComponent
    {
        public WebCamTexture webcamTexture { get; private set; }
        
        void Start()
        {
            webcamTexture = new WebCamTexture();
            
            // Load selected camera from PlayerPrefs
            string deviceName = PlayerPrefs.GetString("WebCamDeviceName");

            webcamTexture.deviceName = deviceName != "" ? deviceName : WebCamTexture.devices[0].name;
            
            if (webcamTexture.deviceName != "")
                webcamTexture.Play();
        }
            
        public void RenderDebugUI()
        {
            // Select camera from list
            GUILayout.Label("Available cameras:");

            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                if (GUILayout.Button(WebCamTexture.devices[i].name))
                {
                    webcamTexture.Stop();
                    webcamTexture.deviceName = WebCamTexture.devices[i].name;
                    webcamTexture.Play();
                    
                    // Save selected camera to PlayerPrefs
                    PlayerPrefs.SetString("WebCamDeviceName", WebCamTexture.devices[i].name);
                }
            }
            
            GUILayout.Space(10);
            
            // Info about selected camera
            GUILayout.Label("Selected camera: " + webcamTexture.deviceName);
            GUILayout.Label("Camera resolution: " + webcamTexture.width + "x" + webcamTexture.height);
            GUILayout.Label("Camera FPS: " + webcamTexture.requestedFPS);
            GUILayout.Label("Camera isPlaying: " + webcamTexture.isPlaying);
            
            GUILayout.Box(webcamTexture);
        }
        
        public Texture2D TakePhoto()
        {
            Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
            photo.SetPixels(webcamTexture.GetPixels());
            photo.Apply();
            
            return photo;
        }
        
        public void ShowPreview(RawImage preview)
        {
            preview.texture = webcamTexture;
        }

        public void OnDisable()
        {
            webcamTexture.Stop();
        }
    }
}