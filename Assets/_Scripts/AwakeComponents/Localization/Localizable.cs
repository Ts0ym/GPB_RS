using System;
using System.Collections;
using System.Collections.Generic;
using AwakeComponents.Localization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwakeComponents.Localization
{
    /// <summary>
    /// <c>Localizable</c> is a base class for components that can be localized.
    /// </summary>
    /// <remarks>Used in combination with <c>LocaleManager</c> to localize components.</remarks>
    /// <seealso cref="LocaleManager"/>
    /// <seealso cref="Language"/>
    public class Localizable : MonoBehaviour
    {
        /// <summary>
        /// Localizes the component based on the current <see cref="Language"/> right after it is enabled.
        /// </summary>
        private void OnEnable()
        {
            Localize(LocaleManager.CurrentLanguage);
        }

        public virtual void Localize(Language language)
        {
        }
    }
}