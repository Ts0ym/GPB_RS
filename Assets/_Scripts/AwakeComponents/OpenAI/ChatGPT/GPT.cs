using System;
using System.Collections;
using System.Linq;
using AwakeComponents.ChatGPT.DataTypes;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Message = AwakeComponents.ChatGPT.DataTypes.Message;

namespace AwakeComponents.ChatGPT
{
    [ComponentInfo("1.0.4", "16.04.2024")]
    public class GPT : MonoBehaviour, IDebuggableComponent
    {
        public string proxyUrl = "http://77.221.133.56/OpenAIProxy.php";
        
        public Settings settings = new();
        
        // Приветственное сообщение модели по умолчанию
        [TextArea(3, 10)]
        public string welcomeMessage = "";
        
        private MessageList context = new();
        
        public UnityEvent<string> onResponse = new();
        public UnityEvent<string> onEndConversation = new();
        
        public int maxAssistantMessages = 8;
        public int maxOutgoingMessageLength = 70;

        public void Start()
        {
            int _maxAssistantMessages = PlayerPrefs.GetInt("GPT.maxAssistantMessages", 0);
            if (_maxAssistantMessages != 0)
                maxAssistantMessages = _maxAssistantMessages;
            
            int _maxOutgoingMessageLength = PlayerPrefs.GetInt("GPT.maxOutgoingMessageLength", 0);
            if (_maxOutgoingMessageLength != 0)
                maxOutgoingMessageLength = _maxOutgoingMessageLength;

            if (welcomeMessage != "")
            {
                Message welcome = new()
                {
                    role = "assistant",
                    content = welcomeMessage
                };
                
                OnGPTResponse(JsonUtility.ToJson(welcome), null);
            }
        }
        
        public void Ask(string text) => Ask(text, null, null);

        /// <summary>Отправляет сообщение и получает ответ, который вернется в callback, когда придет от сервера</summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="callback"><see cref="Action"/>, который вызовется когда придет ответ</param>
        /// <param name="settings">Настройки <see cref="Settings"/> для отправки сообщения</param>
        public void Ask(string text, Action<string> callback, Settings chatSettings = null)
        {
            if (text.Length > maxOutgoingMessageLength) text = text[..maxOutgoingMessageLength];

            chatSettings ??= settings;
            
            var outgoingMessage = new Message
            {
                role = "user",
                content = text
            };
            
            context.messages.Add(outgoingMessage);

            var form = MakeRequestForm(chatSettings, context);
            
            StartCoroutine(PostRequest(proxyUrl, form, gptResponse =>
            {
                try
                {
                    OnGPTResponse(gptResponse, callback);
                }
                catch (ArgumentException e)
                {
                    OnError(gptResponse, callback, e);
                }
            }));
        }
        
        void OnGPTResponse(string gptResponse, Action<string> callback)
        {
            var responseMessage = JsonUtility.FromJson<Message>(gptResponse);
            context.messages.Add(responseMessage);

            onResponse.Invoke(responseMessage.content);
            callback?.Invoke(gptResponse);

            if (GetAssistantMessagesCount() >= maxAssistantMessages)
            {
                onEndConversation.Invoke("Чат завершен");
            }
        }
        
        void OnError(string gptResponse, Action<string> callback, ArgumentException e)
        {
            Debug.LogError("Error while parsing response: " + e.Message);
            Debug.LogError("Response: " + gptResponse);

            var errorMessage = new Message {role = "system", content = "{ \"message\":\"ЧАТ ЗАКРЫТ\", \"error\": \"" + e.Message + "\", \"response\": \"" + gptResponse + "\" }"};

            context.messages.Add(errorMessage);
            onResponse.Invoke("ERROR: 0x48129735");
            callback?.Invoke(gptResponse);

            if (GetAssistantMessagesCount() >= maxAssistantMessages)
            {
                onEndConversation.Invoke("Чат завершен");
            }
        }
        
        private int GetAssistantMessagesCount() => context.messages.Count(message => message.role == "assistant");

        IEnumerator PostRequest(string url, WWWForm form, Action<string> onResponse)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                onResponse?.Invoke(uwr.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
        }
        
        
        public WWWForm MakeRequestForm(Settings chatSettings, MessageList chatContext)
        {
            WWWForm form = new WWWForm();

            form.AddField("settings", JsonUtility.ToJson(chatSettings));
            form.AddField("messages", JsonUtility.ToJson(chatContext));
            
            return form;
        }

        
        private string testMessage;
        public void RenderDebugUI ()
        {
            // Edit settings
            
            GUILayout.Label("Максимальное кол-во вводимых символов:");
            maxOutgoingMessageLength = int.Parse(GUILayout.TextField(maxOutgoingMessageLength.ToString()));
            
            GUILayout.Label("Максимальное кол-во сообщений от AI:");
            maxAssistantMessages = int.Parse(GUILayout.TextField(maxAssistantMessages.ToString()));
            
            GUILayout.Space(10);
            if (GUILayout.Button("Сохранить настройки"))
                SaveToPrefs();
            
            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            
            GUILayout.Label("НАСТРОЙКИ МОДЕЛИ");
            
            GUILayout.Label("Температура: " + settings.temperature);
            settings.temperature = GUILayout.HorizontalSlider((float) settings.temperature, 0.0f, 1.0f);
            
            GUILayout.BeginVertical(GUILayout.Width(270));
            
            GUILayout.Label("Инструкции модели:");
            
            settings.instruction = GUILayout.TextArea(settings.instruction);
            
            GUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            // Send message
            
            GUILayout.Label("ДИАЛОГ С МОДЕЛЬЮ");

            GUILayout.BeginHorizontal();

            testMessage = GUILayout.TextField(testMessage);

            if (GUILayout.Button("Send"))
                Ask(testMessage);
            
            GUILayout.EndHorizontal();

            // View dialog

            for (int i = 0; i < context.messages.Count; i++)
                GUILayout.Label(context.messages[i].role + ": " + context.messages[i].content);
        }

        private void SaveToPrefs()
        {
            PlayerPrefs.SetInt("GPT.maxOutgoingMessageLength", maxOutgoingMessageLength);
            PlayerPrefs.SetInt("GPT.maxAssistantMessages", maxAssistantMessages);
        }
    } 
}

