using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeComponents.ChatGPT.DataTypes
{
    [System.Serializable]
    public class Settings
    {
        // Параметр, который определяет степень случайности в генерации ответа
        [Range(0, 3)]
        public double temperature = 0.7;
        // Какую модель GPT-3 использовать для генерации ответа
        public string engine = "gpt-3.5-turbo";
        // Максимальное количество токенов (слов или символов) в ответе
        [Range(1, 512)]
        public int max_tokens = 300;
        // Количество вариантов ответа, которые будут сгенерированы
        public int n = 1;
        // Список слов или фраз, при наличии которых генерация ответа должна быть завершена
        public string[] stop = new []{"stop now"};

        [TextArea(3, 10)] 
        public string instruction = "";
    }
    
    
    // TODO: Отсюда лучше убрать textObject, он не нужен
    // TODO: И перенести его в класс MessageUI
    // TODO: Почему: у Message не обязательно есть UI-элемент, это просто сообщение ChatGPT
    [System.Serializable]
    public class Message
    {
        public string role;
        [TextArea(3, 10)]
        public string content;
    }
    
    [System.Serializable]
    public class MessageList
    {
        public List<Message> messages = new List<Message>();
    }
        
    [System.Serializable]
    public enum Role
    {
        System,
        User,
        Assistant
    }
    
    
}


