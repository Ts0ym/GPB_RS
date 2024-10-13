using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AwakeComponents.Censorship
{
    public class ChatGptCensor : MonoBehaviour
    {
        public CensorshipManager censorshipManager;
        public UnityEvent<string> onValidationFail;

        public void OnNewMessage(ChatGPT.DataTypes.Message message)
        {
            if (!censorshipManager.Validate(message.content))
            {
                onValidationFail.Invoke(message.content);
            }
        }
    }
}