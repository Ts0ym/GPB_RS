using AwakeComponents.Localization;
using AwakeComponents.StreamingAssetsManager;
using UnityEngine;

namespace AwakeComponents.AwakeMediaPlayer
{
    /// <summary>
    /// Localizator for AwakeMediaPlayer.
    /// </summary>
    [RequireComponent(typeof(AMP))]
    public class AMPLocalizator : Localizable
    {
        AMP amp;

        public void Awake()
        {
            amp = GetComponent<AMP>();
            
            LocaleManager.onLanguageChanged.AddListener(Localize);
        }
        
        public override void Localize(Language language)
        {
            if (amp.debug) Debug.Log("[AMPLocalizator] Localizing AMP: " + amp.folderPath + "/" + amp.fileName);

            amp.Open(amp.folderPath, amp.fileName, amp.autoplay, amp.Loop);
        }

        /// <summary>
        /// Localizes the file name based on the current language.
        /// <br/>
        /// If the file name with the current language code exists in the folder, it returns the new file name. <br/>
        /// If it doesn't, it returns the original file name.
        /// </summary>
        /// <param name="folderPath"><see cref="AMP.folderPath"/></param>
        /// <param name="fileName"><see cref="AMP.fileName"/></param>
        /// <returns>Localized file name.</returns>
        public string LocalizeFileName(string folderPath, string fileName)
        {
            // This method checks if fileName + "_" + language.code exists in the folderPath
            // If it does, it returns the new fileName
            // If it doesn't, it returns the original fileName
            
            if (amp.debug) Debug.Log("[AMPLocalizator] Localizing file name: " + folderPath + "/" + fileName);
            
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("[AMPLocalizator] File name is null or empty");
                return null;
            }
            
            bool fileExists = SA.Find(folderPath, fileName + "_" + LocaleManager.CurrentLanguage.code) != null;
            
            return fileExists ? fileName + "_" + LocaleManager.CurrentLanguage.code : fileName;
        }
    }
}