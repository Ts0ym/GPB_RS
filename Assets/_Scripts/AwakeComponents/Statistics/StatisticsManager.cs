using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.Networking;

namespace AwakeComponents.Statistics
{
    [ComponentInfo("0.6.2", "06.04.2024")]
    public class StatisticsManager : MonoBehaviour, IDebuggableComponent
    {
        public string appInstanceName = "";

        public bool activeInEditor = true;
        public bool isDebug = false;

        static string serverURL = "https://stats.awake.su/api/event/store";
        static string statsVersion;

        public static StatisticsManager instance;

        private bool isSendingBulk = false;

        void Start()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            instance = this;

            statsVersion = this.GetType().GetCustomAttribute<ComponentInfoAttribute>().Version;

            Store("default.started");

            StartCoroutine(Ping());
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Store("default.clicked");
            else if (Input.anyKeyDown)
                Store("default.key_pressed");
        }

        IEnumerator Ping()
        {
            while (true)
            {
                Store("default.ping");
                yield return new WaitForSeconds(60f);
            }
        }

        void OnApplicationQuit()
        {
            Store("default.quit");
        }

        public static void Store(string eventName)
        {
            string platform = "Unknown";

#if UNITY_EDITOR
            platform =
                "Editor"
                + " ("
                + SystemInfo.operatingSystem
                + ", "
                + SystemInfo.deviceModel
                + ", "
                + SystemInfo.deviceName
                + ", "
                + SystemInfo.deviceType
                + ", "
                + SystemInfo.graphicsDeviceName
                + ", "
                +
                //SystemInfo.graphicsDeviceType + ", " +
                //SystemInfo.graphicsDeviceVendor + ", " +
                //SystemInfo.graphicsDeviceVendorID + ", " +
                //SystemInfo.graphicsDeviceVersion + ", " +
                //SystemInfo.graphicsMemorySize + ", " +
                //SystemInfo.graphicsMultiThreaded + ", " +
                //SystemInfo.graphicsShaderLevel + ", " +
                //SystemInfo.maxTextureSize + ", " +
                //SystemInfo.processorCount + ", " +
                //SystemInfo.processorType + ", " +
                //SystemInfo.supportsGyroscope + ", " +
                //SystemInfo.supportsVibration + ", " +
                SystemInfo.systemMemorySize
                + ")";
#elif UNITY_ANDROID
            platform = "Android";
#elif UNITY_IOS
            platform = "iOS";
#elif UNITY_STANDALONE_OSX
            platform = "MacOS";
#elif UNITY_STANDALONE_WIN
            platform = "Windows";
#endif

            string appName = Application.productName;

            if (instance.appInstanceName != "")
                appName += "." + instance.appInstanceName;

            var eventMessage = new StatEventMessage(
                appName: appName,
                appVersion: Application.version,
                eventTime: DateTime.UtcNow.ToString(),
                eventName: eventName,
                statsVersion: statsVersion,
                platform: platform
            );

            string jsonMessage = JsonUtility.ToJson(eventMessage);

#if UNITY_EDITOR
            if (!instance.activeInEditor)
                return;
#endif

            SendStat(jsonMessage);
#if UNITY_EDITOR
            if (instance.isDebug)
                Debug.Log("[AwakeStats] Counted: " + jsonMessage);
#endif
        }

        static void SendStat(string jsonMessage)
        {
            var awakeStats = FindObjectOfType<StatisticsManager>();

            if (awakeStats == null)
            {
                Debug.LogError("[AwakeStats] AwakeStats component is not found! Fix this!");
                return;
            }

            awakeStats.StartCoroutine(awakeStats._SendStat(jsonMessage));
        }

        IEnumerator _SendStat(string jsonMessage)
        {
            WWWForm form = new WWWForm();
            form.AddField("eventJson", jsonMessage);

            UnityWebRequest www = UnityWebRequest.Post(serverURL, form);

            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"Failed to send stat: {www.error}";

                if (instance.isDebug)
                    Debug.LogError(errorMessage);

                // Если это не ping
                if (!jsonMessage.Contains("default.ping"))
                    // Записываем в файл
                    WriteErrorToFile(jsonMessage);
            }
            else
            {
#if UNITY_EDITOR
                if (instance.isDebug)
                    Debug.Log($"Stat sent successfully: {www.downloadHandler.text}");
#endif

                // Если успешно отправили, то чекаем неудачные сообщения
                CheckUnsuccessfulStats();
            }

            www.disposeUploadHandlerOnDispose = true;
            www.disposeDownloadHandlerOnDispose = true;

            www.uploadHandler.Dispose();
            www.downloadHandler.Dispose();

            www.Dispose();
        }

        void CheckUnsuccessfulStats()
        {
            string filePath = Application.persistentDataPath + "/unsuccessful_stats_requests.txt";

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string[] lines = System.IO.File.ReadAllLines(filePath);

                    if (lines.Length > 100)
                    {
                        if (!isSendingBulk)
                        {
                            string bulkData = "[" + String.Join(",", lines) + "]";
                            StartCoroutine(_SendBulkStat(bulkData, filePath));
                        }
                    }
                    else if (lines.Length > 0)
                    {
                        // Existing behavior for less than 100 lines
                        string firstLine = lines[0];
                        StartCoroutine(_SendStat(firstLine));

                        List<string> remainingLines = new List<string>(lines);
                        remainingLines.RemoveAt(0);
                        System.IO.File.WriteAllLines(filePath, remainingLines.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to check unsuccessful stats: {e.Message}");
            }
        }

        IEnumerator _SendBulkStat(string bulkData, string filePath)
        {
            isSendingBulk = true;

            WWWForm form = new WWWForm();
            form.AddField("bulkEventJson", bulkData);

            UnityWebRequest www = UnityWebRequest.Post(
                "https://stats.awake.su/api/event/storeArray",
                form
            );

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                if (instance.isDebug)
                    Debug.LogError($"Failed to send bulk stats: {www.error}");
#endif
            }
            else
            {
                // Delete the file after successful submission
                System.IO.File.Delete(filePath);
#if UNITY_EDITOR
                if (instance.isDebug)
                    Debug.Log("Bulk stats sent successfully.");
#endif
            }

            isSendingBulk = false;
        }

        static void WriteErrorToFile(string failedMessage)
        {
            string filePath = Application.persistentDataPath + "/unsuccessful_stats_requests.txt";

            try
            {
                System.IO.File.AppendAllText(filePath, failedMessage + "\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write error to file: {e.Message}");
            }
        }

        public void RenderDebugUI()
        {
            GUILayout.Box("Nothing to see here");
        }
    }
}
