using System;
using System.Collections;
using System.Collections.Generic;
using AwakeComponents.DebugUI;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeComponents.Localization
{
    /// <summary>
    /// <c>LocaleManager</c> is a component that manages localization in the application.
    /// </summary>
    [ComponentInfo("1.2", "04.04.2024")]
    public class LocaleManager : MonoBehaviour, IDebuggableComponent
    {
        /// <summary>
        /// List of available languages.
        /// </summary>
        public List<Language> languages = new()
        {
            new Language { code = "ru" },
            new Language { code = "en" }
        }; 

        /// <summary>
        /// Current <see cref="Language"/> of the application.
        /// </summary>
        public static Language CurrentLanguage { get; private set; }
        
        /// <summary>
        /// Event that is invoked when the <see cref="Language"/> is changed.
        /// </summary>
        public static UnityEvent<Language> onLanguageChanged = new();

        /// <summary>
        /// List of <see cref="Localizable"/> components that are registered in the <see cref="LocaleManager"/>.
        /// </summary>
        private List<Localizable> localizables = new();

        private void Awake()
        {
            CurrentLanguage = languages[0];
        }

        /// <summary>
        /// Switches the current <see cref="Language"/> to the next one in the list.
        /// </summary>
        public void NextLanguage()
        {
            Language nextLanguage = languages[(languages.IndexOf(CurrentLanguage) + 1) % languages.Count];
            
            Debug.Log("[Awake.Locale] Language '" + CurrentLanguage + "' switched to '" + nextLanguage + "'!");

            CurrentLanguage = nextLanguage;
            
            onLanguageChanged.Invoke(CurrentLanguage);
        }
        
        /// <summary>
        /// Registers a <see cref="Localizable"/> component in the <see cref="LocaleManager"/>.
        /// </summary>
        /// <param name="localizable">Localizable component to register.</param>
        public void RegisterLocalizable(Localizable localizable)
        {
            localizables.Add(localizable);
        }
        
        /// <summary>
        /// Unregisters a <see cref="Localizable"/> component from the <see cref="LocaleManager"/>.
        /// </summary>
        /// <param name="localizable">Localizable component to unregister.</param>
        public void UnregisterLocalizable(Localizable localizable)
        {
            localizables.Remove(localizable);
        }
        
        public void RenderDebugUI()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Box("Current language: " + CurrentLanguage.code);
            
            GUI.backgroundColor = Color.cyan;
            
            if (GUILayout.Button("Next"))
                NextLanguage();
            
            GUI.backgroundColor = Color.white;
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            GUILayout.Label("Languages: " + string.Join(", ", languages));
            GUILayout.Label("Localizables count: " + localizables.Count);
        }
    }
}
