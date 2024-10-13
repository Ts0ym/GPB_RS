using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AwakeComponents.SoftUI.NavigableUISet
{
    /// <summary>
    /// <c>Item</c> is a component that can be attached to any GameObject with a Button component.
    /// <br/>
    /// It provides a set of events that can be used to handle focus and selection.
    /// </summary>
    /// <remarks>Used in combination with <c>Navigation</c> component to create a list of selectable items.</remarks>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class Item : MonoBehaviour
    {
        /// <summary>
        /// Event that is invoked when the item is focused in.
        /// </summary>
        public UnityEvent onFocusIn = new();
        
        /// <summary>
        /// Event that is invoked when the item is focused out.
        /// </summary>
        public UnityEvent onFocusOut = new();

        /// <summary>
        /// Event that is invoked when the item is selected.
        /// </summary>
        public UnityEvent onSelect = new();

        /// <summary>
        /// Returns true if the item is in focus.
        /// </summary>
        public bool IsInFocus { get; private set; }
        
        private void Awake()
        {
            // Subscribe to UI.Button.onClick event
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Select);
        }

        /// <summary>
        /// Focuses the item in.
        /// </summary>
        public void FocusIn()
        {
            onFocusIn.Invoke();
            IsInFocus = true;
        }

        /// <summary>
        /// Focuses the item out.
        /// </summary>
        public void FocusOut()
        {
            onFocusOut.Invoke();
            IsInFocus = false;
        }

        /// <summary>
        /// Selects the item.
        /// It is called when the Button component is clicked or when the Select method is called from another script, e.g. from ListController.
        /// </summary>
        public void Select()
        {
            onSelect.Invoke();
        }
    }
}