using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AwakeComponents.Censorship
{
    public class InputTextCensor : MonoBehaviour
    {
        public CensorshipManager censorshipManager;
        public UnityEvent<Text> onValidationFail;

        private void Start()
        {
            gameObject.GetComponent<InputField>().onValueChanged.AddListener(delegate { OnInputFieldValueChanged(); });
        }

        public void OnInputFieldValueChanged()
        {
            var textObj = gameObject.GetComponent<InputField>().textComponent;
            var message = textObj.text;
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (!censorshipManager.Validate(textObj.text))
                onValidationFail.Invoke(textObj);
        }
    }
}