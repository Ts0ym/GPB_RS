using System.Collections.Generic;
using AwakeComponents.SoftUI.Animations;
using AwakeComponents.SoftUI.Animations.DataTypes;
using UnityEngine;

namespace AwakeComponents.SoftUI.NavigableUISet.Presets
{
    /// <summary>
    /// This class is used to animate the item when it is focused, selected or deselected. <br/>
    /// It uses the <c>SoftUIAnimator</c> to animate the item. <br/>
    /// The animations are triggered by the <c>Item</c> component events.
    /// </summary>
    public class ItemAnimator : MonoBehaviour
    {
        /// <summary>
        /// The <c>Item</c> component that triggers the animations.
        /// </summary>
        Item _item;
        
        /// <summary>
        /// The <c>SoftUIAnimator</c> component that animates the item.
        /// </summary>
        SoftUIAnimator _softUIAnimator;
    
        /// <summary>
        /// The list of animations to play when the item is started.
        /// </summary>
        [Header("Animations")]
        public List<AnimationPreset> startAnimations = new List<AnimationPreset>();
        /// <summary>
        /// The list of animations to play when the item is focused.
        /// </summary>
        public List<AnimationPreset> onFocusInAnimations = new List<AnimationPreset>();
        /// <summary>
        /// The list of animations to play when the item is unfocused.
        /// </summary>
        public List<AnimationPreset> onFocusOutAnimations = new List<AnimationPreset>();
        /// <summary>
        /// The list of animations to play when the item is selected.
        /// </summary>
        public List<AnimationPreset> onSelectAnimations = new List<AnimationPreset>();

        /// <summary>
        /// Initializes the <c>ItemAnimator</c> component.
        /// </summary>
        void Start()
        {
            _item = GetComponent<Item>();
            _softUIAnimator = GetComponent<SoftUIAnimator>();
        
            if (_item == null)
            {
                Debug.LogError("SelectableItem component not found");
                return;
            }
        
            if (_softUIAnimator == null)
            {
                _softUIAnimator = gameObject.AddComponent<SoftUIAnimator>();
            }
        
            _item.onFocusIn.AddListener(OnFocusIn);
            _item.onFocusOut.AddListener(OnFocusOut);
            _item.onSelect.AddListener(OnSelect);
        
            PlayStartAnimations();
        }

        /// <summary>
        /// Plays the start animations when the item is initialized.
        /// </summary>
        /// <remarks>
        /// It could be used to play the animations when the item is created or to set the initial state of the item.
        /// </remarks>
        private void PlayStartAnimations()
        {
            foreach (var animationPreset in startAnimations)
                _softUIAnimator.Animate(animationPreset.AnimationType, animationPreset.AnimationSpeed);
        }

        /// <summary>
        /// Plays the animations when the item is focused.
        /// </summary>
        void OnFocusIn()
        {
            foreach (var animationPreset in onFocusInAnimations)
            {
                _softUIAnimator.Animate(animationPreset.AnimationType, animationPreset.AnimationSpeed);
            }
        }

        /// <summary>
        /// Plays the animations when the item is unfocused.
        /// </summary>
        void OnFocusOut()
        {
            foreach (var animationPreset in onFocusOutAnimations)
            {
                _softUIAnimator.Animate(animationPreset.AnimationType, animationPreset.AnimationSpeed);
            }
        }
    
        /// <summary>
        /// Plays the animations when the item is selected.
        /// </summary>
        void OnSelect()
        {
            foreach (var animationPreset in onSelectAnimations)
            {
                _softUIAnimator.Animate(animationPreset.AnimationType, animationPreset.AnimationSpeed);
            }
        }
    }
}
