using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AwakeKeyboard : MonoBehaviour
{
    private TMP_InputField currentInputField;
    
    GameObject keyboard;
    
    private void Start()
    {
        keyboard = gameObject;
        
        TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>();

        foreach (var inputField in inputFields)
        {
            EventTrigger eventTrigger = inputField.gameObject.GetComponent<EventTrigger>();

            if (eventTrigger == null)
            {
                eventTrigger = inputField.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnClick(); });

            eventTrigger.triggers.Add(entry);
        }
    }

    private void OnClick()
    {
        Debug.Log("Клик в Input Field with name: " + EventSystem.current.currentSelectedGameObject.name);
        
        currentInputField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        
        if (currentInputField != null)
        {
            ShowKeyboard();
        }
    }
    
    private void ShowKeyboard()
    {
        keyboard.SetActive(true);
    }

    public void OnButtonCLick()
    {
        // Which button was pressed?
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Button pressed: " + buttonName);

        if (buttonName.ToLower() != "ru")
        {
            AppendCharacter(buttonName);
        }
    }

    void AppendCharacter(string buttonName)
    {
        if (currentInputField != null)
        {
            currentInputField.text += buttonName;
        }
    }
}
