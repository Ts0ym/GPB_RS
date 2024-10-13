using System;
using UnityEngine;

namespace AwakeComponents.SoftUI.Animations.DataTypes
{
    [Serializable]
    [SerializeField]
    public enum AnimationType
    {
        #region Alpha
        
        /// <summary>Makes the object opaque from 0 to 1 alpha</summary>
        AlphaIn,
        /// <summary>Makes the object transparent from 1 to 0 alpha</summary>
        AlphaOut,
        /// <summary>Makes the object slightly more opaque from current alpha to alpha + delta</summary>
        AlphaSlightIn,
        /// <summary>Makes the object slightly more transparent from current alpha to alpha - delta</summary>
        AlphaSlightOut,
        /// <summary>Makes the object opaque from current alpha to 1 alpha</summary>
        ToOpaque,
        /// <summary>Makes the object transparent from current alpha to 0 alpha</summary>
        ToTransparent,
        
        #endregion

        #region Position
        
        /// <summary>Moves the object from start position plus delta-downwards to start position</summary>
        SlideUpIn,
        /// <summary>Moves the object from start position plus delta-upwards to start position</summary>
        SlideDownIn,
        /// <summary>Moves the object from start position plus delta-rightwards to start position</summary>
        SlideLeftIn,
        /// <summary>Moves the object from start position plus delta-leftwards to start position</summary>
        SlideRightIn,
        
        /// <summary>Moves the object from start position to start position plus delta-upwards</summary>
        SlideUpOut,
        /// <summary>Moves the object from start position to start position plus delta-downwards</summary>
        SlideDownOut,
        /// <summary>Moves the object from start position to start position plus delta-leftwards</summary>
        SlideLeftOut,
        /// <summary>Moves the object from start position to start position plus delta-rightwards</summary>
        SlideRightOut,

        #endregion
    }
}