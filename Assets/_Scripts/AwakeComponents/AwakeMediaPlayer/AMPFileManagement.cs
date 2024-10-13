using System;
using UnityEngine;
using AwakeComponents.StreamingAssetsManager;

namespace AwakeComponents.AwakeMediaPlayer
{
    /// <summary>
    /// Class responsible for managing file operations in AwakeMediaPlayer.
    /// <br/>
    /// It is used to open files, set files and manage file paths.
    /// </summary>
    
    // ReSharper disable once InconsistentNaming
    public class AMPFileManagement
    {
        private AMP _amp;

        public AMPFileManagement(AMP amp)
        {
            _amp = amp;
        }

        /// <inheritdoc cref="AMP.Open(string, string, bool, bool)"/>
        /// <remarks>Do not use this method directly. Use <see cref="AMP.Open(string, string, bool, bool)"/> instead.</remarks>
        public void Open(string folderPath, string fileName, bool autoplay, bool loop)
        {
            if (_amp.debug) Debug.Log("[AMPFileManagement] Opening file: " + folderPath + "/" + fileName + " autoplay: " + autoplay + " loop: " + loop);
            
            _amp.folderPath = folderPath;
            _amp.fileName = fileName;
            _amp.autoplay = autoplay;
            _amp.Loop = loop;
            
            if (_amp.localizable)
                fileName = _amp.Localizator.LocalizeFileName(folderPath, fileName);

            if (!string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
            {
                string exactPath = SA.Find(folderPath, fileName);

                if (exactPath != null)
                {
                    SetFile(exactPath);
                    
                    if (_amp.autoplay)
                        _amp.Controls.Play();
                }
                else
                    throw new NullReferenceException("[AMPFileManagement] File not found in StreamingAssets: " + folderPath + "/" + fileName);
            } 
            else
            {
                Debug.LogError("[AMPFileManagement] Folder path or file name is null or empty");
            }
        }

        /// <summary>
        /// Sets the file to be played, based on the exact system path relative to the root of the PC.
        /// </summary>
        /// <param name="exactPath">Exact path to the file relative to the root of the PC.</param>
        private void SetFile(string exactPath)
        {
            if (_amp.debug) Debug.Log("[AMPFileManagement] Setting file: " + exactPath);
            
            if (string.IsNullOrEmpty(exactPath))
            {
                Debug.LogError("[AMPFileManagement] Provided path is null or empty");
                return;
            }

            string fileExtension = GetExtensionFromPath(exactPath);
            PlayerType playerType = GetPlayerTypeByExtension(fileExtension);
            _amp.Controls.SetPlayer(playerType, exactPath);
        }

        /// <summary>
        /// Gets the <see cref="PlayerType"/> based on the file extension.
        /// </summary>
        /// <param name="fileExtension">Extension of the file.</param>
        /// <returns><see cref="PlayerType"/> of the file.</returns>
        private PlayerType GetPlayerTypeByExtension(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "mov":
                    return PlayerType.HAP;
                case "mp4":
                case "webm":
                    return PlayerType.VIDEO;
                case "png":
                case "jpg":
                    return PlayerType.IMAGE;
                default:
                    if (_amp.debug) Debug.LogError("[AMPFileManagement] Unrecognized file extension: " + fileExtension);
                    return PlayerType.NONE;
            }
        }

        /// <summary>
        /// Gets the extension of the file based on the path.
        /// </summary>
        /// <param name="path">Full path to the file including the extension.</param>
        /// <returns>Extension of the file.</returns>
        private string GetExtensionFromPath(string path)
        {
            int dotIndex = path.LastIndexOf('.');
            if (dotIndex == -1 || dotIndex == path.Length - 1)
            {
                return "";
            }

            return path.Substring(dotIndex + 1);
        }
    }
    
    /// <summary>
    /// Type of media file:
    /// <list type="bullet">
    /// <item><description><c>NONE</c>: No media loaded or wrong type</description></item>
    /// <item><description><c>IMAGE</c>: Image files JPG / PNG</description></item>
    /// <item><description><c>VIDEO</c>: Video files MP4 / WEBM</description></item>
    /// <item><description><c>HAP</c>: HAP encoded MOV video files</description></item>
    /// </list>
    /// </summary>
    public enum PlayerType { NONE, IMAGE, VIDEO, HAP }
}