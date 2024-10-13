using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeComponents.ChatUI
{
    public class MessageTextAnimator : MonoBehaviour
    {
        private Message message;
        private string sourceString;
        
        public float delay = 0.01f;
        
        void Start()
        {
            message = GetComponent<Message>();
            sourceString = message.text;
            message.text = "";
            
            StartCoroutine(ShowText());
        }
        
        IEnumerator ShowText()
        {
            string resultString = "";
            
            while (sourceString.Length > 0) {
                resultString += sourceString.Substring(0, 1);
                sourceString = sourceString[1..^0];

                message.text = resultString;

                message.chat.ViewUpdateAndScroll();

                yield return new WaitForSeconds(delay);
            }
        }
    }
}