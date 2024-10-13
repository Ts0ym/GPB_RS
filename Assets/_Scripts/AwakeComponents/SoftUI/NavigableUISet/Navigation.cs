using System;
using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using AwakeComponents.Utils;
using UnityEngine;

namespace AwakeComponents.SoftUI.NavigableUISet
{
    /// <summary>
    /// Component that manages a list of <c>SelectableItem</c> components.
    /// <br/><br/>
    /// It allows to navigate through items and select them using <c>Navigate</c> and <c>SelectItem</c> methods.
    /// </summary>
    /// <remarks>
    /// Can be used to create a list of selectable items, like a menu or a list of options where only one item can be selected at a time and navigated with arrow keys, RotarySC or other input methods.
    /// </remarks>
    [ComponentInfo("1.0.1", "05.04.2024")]
    public class Navigation : MonoBehaviour, IDebuggableComponent
    {
        /// <summary>
        /// List of SelectableItems. It is filled with all SelectableItems in children.
        /// </summary>
        private readonly List<Item> _items = new();

        /// <summary>
        /// Index of the currently focused item.
        /// </summary>
        private int _focusedItemIndex = 0;
        
        /// <summary>
        /// If true, the focus will loop through items when reaching the end of the list or the beginning.
        /// </summary>
        public bool isLooped = true;

        void Start()
        {
            foreach (var item in GetComponentsInChildren<Item>())
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Navigates through the items in the list.
        /// </summary>
        /// <param name="direction">Direction of the navigation.</param>
        public void Navigate(CustomDataTypes.Direction direction)
        {
            if (gameObject.activeInHierarchy)
                SwitchFocus(direction);
        }

        /// <summary>
        /// Switches the focus to the next or previous item in the list.
        /// </summary>
        /// <param name="direction">Direction of the navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the direction is not valid.</exception>
        private void SwitchFocus(CustomDataTypes.Direction direction)
        {
            switch (direction)
            {
                case CustomDataTypes.Direction.Left:
                    FocusItem(_focusedItemIndex - 1);
                    break;
                case CustomDataTypes.Direction.Right:
                    FocusItem(_focusedItemIndex + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary>
        /// Focuses the item at the given index.
        /// </summary>
        /// <param name="index">Index of the item to focus.</param>
        private void FocusItem(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                if (isLooped)
                    index = index < 0 ? _items.Count - 1 : 0;
                else
                    return;
            }

            _items[_focusedItemIndex].FocusOut();

            _focusedItemIndex = index;

            _items[_focusedItemIndex].FocusIn();
        }

        /// <summary>
        /// Selects the currently focused item.
        /// </summary>
        public void SelectItem()
        {
            if (gameObject.activeInHierarchy)
                _items[_focusedItemIndex].Select();
        }
        
        /// <summary>
        /// Gets the UI of the component for debugging purposes using DebugUI.
        /// </summary>
        public void RenderDebugUI()
        {
            GUILayout.Label("Is active: " + gameObject.activeInHierarchy);
            GUILayout.Label("Is looped: " + isLooped);
            GUILayout.Label("Items count: " + _items.Count);
            GUILayout.Label("Focused item index: " + _focusedItemIndex);
        }
    }
}