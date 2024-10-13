using System;
using Unity.VisualScripting;
using UnityEngine;

namespace AwakeComponents.SoftUI.Animations.DataTypes
{
    [Serializable]
    public class AnimationPreset
    {
        [Serialize] [SerializeField] public AnimationType AnimationType;
        [Serialize] [SerializeField] public AnimationSpeed AnimationSpeed;
    }
}