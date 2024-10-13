using System;
using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwakeComponents.ChatUI
{
    [ComponentInfo("1.1.1", "16.04.2024")]
    public class Chat : MonoBehaviour, IDebuggableComponent
    {
        public Transform content;
        public GameObject messagePrefab;
        public InputField inputField;
        public Button sendButton;
        public ScrollRect scrollRect;
        private RectTransform contentRectTransform;

        public List<Message> messages = new();

        public int maxOutgoingMessageLength = 100;
        
        public bool animateIncomingMessagesText = true;
        public float incomingMessageTextAnimationDelay = 0.01f;

        public UnityEvent<string> onSendMessage = new();
        
        void Start()
        {
            inputField.characterLimit = maxOutgoingMessageLength;
            contentRectTransform = content.GetComponent<RectTransform>();
            
            inputField.onEndEdit.AddListener(OnEndEdit);
        }
        
        private void OnEndEdit(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OnClickSend();
        }
        
        private void OnClickSend()
        {
            var text = inputField.text.Trim();
            
            if (string.IsNullOrEmpty(text))
                return;
            
            AddOutgoingMessage(text);

            onSendMessage.Invoke(text);
            
            inputField.text = "";
        }
        
        public void AddIncomingMessage(string text)
        {
            AddMessage(Message.Direction.Incoming, text);
        }
        
        public void AddOutgoingMessage(string text)
        {
            AddMessage(Message.Direction.Outgoing, text);
        }
        
        public void AddMessage(Message.Direction direction, string text)
        {
            // Запоминаем текущее смещение от верхнего края
            var targetOffsetTop = GetOffsetTop();
            
            // Создаем новое сообщение
            var message = Instantiate(messagePrefab, content).GetComponent<Message>();
            
            // Устанавливаем текст и направление сообщения
            message.direction = direction;
            message.text = text;
            
            // Устанавливаем ссылку на чат
            message.chat = this;

            // Добавляем сообщение в список
            messages.Add(message);
            
            // Устанавливаем смещение от верхнего края
            message.SetOffsetTop(targetOffsetTop);
            
            // Если сообщение входящее и включена анимация текста
            if (direction == Message.Direction.Incoming && animateIncomingMessagesText)
            {
                // Запускаем анимацию текста
                var messageTextAnimator = message.gameObject.AddComponent<MessageTextAnimator>();
                messageTextAnimator.delay = incomingMessageTextAnimationDelay;
            }
            
            // Обновляем размер контента и скроллим вниз
            ViewUpdateAndScroll();
            
            // Включаем кнопку отправки сообщения
            sendButton.interactable = true;
            
            // Передаем фокус на поле ввода
            inputField.Select();
            inputField.ActivateInputField();
        }
        
        public void ViewUpdateAndScroll() => StartCoroutine(ViewUpdateAndScrollCoroutine());

        private IEnumerator ViewUpdateAndScrollCoroutine()
        {
            yield return new WaitForEndOfFrame();
            UpdateContentSize();
            ScrollToBottom();
        }
        
        public void UpdateContentSize() => contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, -GetOffsetTop());
        public void ScrollToBottom() => scrollRect.normalizedPosition = new Vector2(0, 0);

        float GetOffsetTop()
        {
            if (messages.Count == 0)
            {
                return 0;
            }
            
            var lastMessage = messages[^1];
            var lastMessageRectTransform = lastMessage.GetComponent<RectTransform>();
            return lastMessageRectTransform.anchoredPosition.y - lastMessageRectTransform.rect.height;
        }
        
        public void RenderDebugUI()
        {
            GUILayout.Label($"Chat: {gameObject.name}");
            GUILayout.Label($"Messages count: {messages.Count}");
            GUILayout.Label($"Content size: {contentRectTransform.sizeDelta}");
            GUILayout.Label($"Scroll position: {scrollRect.normalizedPosition}");
            
            GUILayout.Space(10);
            
            if (messages.Count > 0)
            {
                GUILayout.Label("Messages:");
                foreach (var message in messages)
                {
                    GUILayout.Label($"- {message.direction}: {message.text}");
                }
            }
        }
    }
}


