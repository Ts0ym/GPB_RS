using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AwakeComponents.StreamingAssetsManager
{
    public static class SA
    {
        #if UNITY_EDITOR
            private const bool IsDebug = true;
        #else
            const bool IsDebug = true;
        #endif
        
        /// <summary>
        /// Find a path of file in the StreamingAssets folder
        /// If more than one file is found, the first one is returned.
        /// </summary>
        /// <param name="location">The location for searching the file in the StreamingAssets folder</param>
        /// <param name="fileName">The name of the file to find (without extension)</param>
        /// <returns> The path to the file, or null if not found. If more than one file is found, the first one is returned.</returns>
        public static string Find(string location, string fileName)
        {
            string[] foundFiles = {};

            if (fileName == "")
            {
                if (IsDebug) Debug.LogWarning("[SA] File name not specified: " + location);
                return null;
            }

            try
            {
                foundFiles = Directory.GetFiles(Application.streamingAssetsPath + "/" + location, fileName + ".*", SearchOption.TopDirectoryOnly);
                foundFiles = foundFiles.Where(FilterFileName).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                if (IsDebug) Debug.LogError("[SA] Directory not found: " + location + ", " + fileName);
            }

            switch (foundFiles.Length)
            {
                case 0:
                    if (IsDebug) Debug.LogWarning("[SA] Files not found: " + location + ", " + fileName);
                    return null;
                case > 1:
                    if (IsDebug) Debug.LogWarning("[SA] Files found more than one: " + location + ", " + fileName);
                    return foundFiles[0];
                default:
                    return foundFiles[0];
            }
        }

        /// <summary>
        /// This method returns all paths of files in the target StreamingAssets folder
        /// </summary>
        /// <param name="location">The location for listing files in the StreamingAssets folder</param>
        /// <returns>The paths to the files, or empty array if not found</returns>
        public static string[] ListAllFiles(string location)
        {
            string[] foundFiles = {};

            try
            {
                foundFiles = Directory.GetFiles(Application.streamingAssetsPath + "/" + location, "*", SearchOption.TopDirectoryOnly);
                foundFiles = foundFiles.Where(FilterFileName).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                if (IsDebug) Debug.LogError("[SA] Directory not found: " + location);
            }

            return foundFiles;
        }

        public static bool Exists(string path)
        {
            bool isExists = false;
            
            Debug.Log("[SA] Checking if exists: " + path);

            path = Application.streamingAssetsPath + "/" + path;

            isExists = File.Exists(path);

            if (!isExists)
                isExists = Directory.Exists(path);
            
            return isExists;
        }

        /// <summary>
        /// This method filters out unwanted files from the list of files
        /// </summary>
        /// <param name="name">The name of the file to filter out</param>
        /// <returns>True if the file is not filtered out, false otherwise</returns>
        static bool FilterFileName(string name)
        {
            return
                !name.ToLower().EndsWith(".meta") &&
                !name.Contains("DS_Store") &&
                !name.Contains("/.") &&
                !name.Contains("\\.");
        }

        public static IEnumerable<string> GetSubdirectories(string currentPath, bool isRelative = true)
        {
            currentPath = @Application.streamingAssetsPath + @"/" + @currentPath;

            string[] subDirs = Directory.GetDirectories(@currentPath);

            if (isRelative)
                for (int i = 0; i < subDirs.Length; i++)
                    subDirs[i] = subDirs[i].Split("/")[^1].Split("\\")[^1];

            return subDirs;
        }
    }
}
