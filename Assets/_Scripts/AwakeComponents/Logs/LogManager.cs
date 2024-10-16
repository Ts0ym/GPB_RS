using System;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

namespace AwakeComponents.Log
{
    [ComponentInfo("0.1", "30.03.2024")]
    public class LogManager : MonoBehaviour, IDebuggableComponent
    {
        // This component subscribes to Debug.Log, LogError, LogWarning and so on
        // and stores the messages in a list.

        private readonly List<LogEntry> _log = new();
        
        void Awake()
        {
            Application.logMessageReceived += Add;
        }
    
        void Add(string condition, string stackTrace, LogType type)
        {
            _log.Add(new LogEntry(DateTime.Now, condition, stackTrace, type));
            
            // Limit the log to 100 entries
            if (_log.Count > 100)
                _log.RemoveAt(0);
        }

        private void Start()
        {
            // Debug.Log("Test");
        }

        public void RenderDebugUI()
        {
            GUILayout.BeginHorizontal();
            
            GUI.backgroundColor = Color.red;
            
            if (GUILayout.Button("Clear log"))
                _log.Clear();
            
            GUI.backgroundColor = new Color(0f, 0.8f, 1f);
            
            if (GUILayout.Button("All"))
                Debug.LogWarning("Not implemented");

            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button("Warning"))
                Debug.LogWarning("Not implemented");
            
            GUI.backgroundColor = new Color(1f, 0.5f, 0f);
            
            if (GUILayout.Button("Error"))
                Debug.LogWarning("Not implemented");
            
            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            
            foreach (var message in (_log.AsEnumerable() ?? Array.Empty<LogEntry>()).Reverse().Take(50))
            {
                GUI.backgroundColor = message.Type switch
                {
                    LogType.Error => Color.red,
                    LogType.Warning => Color.yellow,
                    LogType.Assert => new Color(1f, 0.5f, 0f),
                    LogType.Exception => new Color(1f, 0.5f, 0f),
                    _ => Color.white
                };

                if (GUILayout.Button(message.Message))
                {
                    if (message.Message.Contains(message.StackTrace))
                        message.Message = message.Message.Replace("\n(" + message.Time + ")\n" + message.StackTrace, "");
                    else
                        message.Message += "\n(" + message.Time + ")\n" + message.StackTrace;
                }
            }
        }
    }
}
