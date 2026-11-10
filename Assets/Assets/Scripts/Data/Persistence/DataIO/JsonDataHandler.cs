using System;
using System.IO;
using System.Linq;
using Assets.Scripts.Data.Domain;
using UnityEngine;

#if !UNITY_EDITOR
using System.Text;
#endif

namespace Assets.Scripts.Data.Persistence.DataIO
{
    public class JsonDataHandler : MonoBehaviour, DataIOHandler
    {
        private string _saveDir;

        private void Awake()
        {
            _saveDir = Path.Combine(Application.persistentDataPath, "Saves");
            #if UNITY_EDITOR
            _saveDir = Path.Combine(_saveDir, "Dev");
            #endif
        }

        public SaveFile[] GetSaves()
        {
            try
            {
                CreateSaveDirectoryIfNotPresent();
                
                return new DirectoryInfo(_saveDir)
                    .GetFiles("*.json")
                    .OrderByDescending(f => f.LastWriteTime)
                    .Select(fileInfo => new SaveFile(fileInfo))
                    .ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error finding saves from path \"{_saveDir}\": {e.Message}");
            }

            return Array.Empty<SaveFile>();
        }

        public GameData Load(string saveName)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveName);
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"Save file \"{saveName}\" does not exist.");
                    return null;
                }

                string dataToLoad = File.ReadAllText(fullPath);

                #if !UNITY_EDITOR
                dataToLoad = Deobfuscate(dataToLoad);
                #endif
                
                return JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading data from path \"{fullPath}\": {e.Message}");
                return null;
            }
        }

        public void Save(GameData data, string saveName)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveName);
                CreateSaveDirectoryIfNotPresent();
            
                string dataToSave = JsonUtility.ToJson(data, true);
                
                #if !UNITY_EDITOR
                dataToSave = Obfuscate(dataToSave);
                #endif

                File.WriteAllText(fullPath, dataToSave);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving data to path \"{fullPath}\": {e.Message}");
            }
        }
        
        private void CreateSaveDirectoryIfNotPresent()
        {
            if (!Directory.Exists(_saveDir)) 
                Directory.CreateDirectory(_saveDir);
        }
        
        private string GetSaveFilePath(string saveName)
        {
            string safeName = SanitizeFileName();
            return Path.Combine(_saveDir, $"{safeName}.json");
            
            string SanitizeFileName()
            {
                if (string.IsNullOrWhiteSpace(saveName))
                    throw new ArgumentException("Save name cannot be empty.");

                char[] invalidChars = Path.GetInvalidFileNameChars();
                string sanitized =
                    string.Join("_", saveName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                        .TrimEnd('.');

                return string.IsNullOrWhiteSpace(sanitized) 
                    ? throw new ArgumentException("Invalid save name.") 
                    : sanitized;
            }
        }

        #if !UNITY_EDITOR
        private static readonly byte[] Key = 
            Encoding.UTF8.GetBytes("KoiKat_Save_Obfuscation_v1");

        private static string Obfuscate(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= Key[i % Key.Length];
            }

            return Convert.ToBase64String(bytes);
        }

        private static string Deobfuscate(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= Key[i % Key.Length];
            }

            return Encoding.UTF8.GetString(bytes);
        }
        #endif
    }
}