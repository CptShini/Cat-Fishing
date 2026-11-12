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
        private const string SAVE_FILE_EXT = ".catfish";
        
        private string _saveDir;

        private void Awake()
        {
            _saveDir = Path.Combine(Application.persistentDataPath, "Saves");
            #if UNITY_EDITOR
            _saveDir = Path.Combine(_saveDir, "Dev");
            #endif
            
            if (!Directory.Exists(_saveDir)) 
                Directory.CreateDirectory(_saveDir);
        }

        public SaveFile[] GetSaves()
        {
            try
            {
                return new DirectoryInfo(_saveDir)
                    .GetFiles($"*{SAVE_FILE_EXT}")
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

        public SaveFile Create(string saveName)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveName);
                using FileStream fs = File.Create(fullPath);
                return new(new(fs.Name));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating save file at path \"{fullPath}\": {e.Message}");
            }

            return null;
        }

        public GameData Load(SaveFile saveFile)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveFile.Name);
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"Save file \"{saveFile.Name}\" does not exist.");
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

        public void Save(GameData data, SaveFile saveFile)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveFile.Name);
            
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

        public void Delete(SaveFile saveFile)
        {
            string fullPath = string.Empty;

            try
            {
                fullPath = GetSaveFilePath(saveFile.Name);
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"Save file \"{saveFile.Name}\" does not exist.");
                    return;
                }

                File.Delete(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting save from path \"{fullPath}\": {e.Message}");
            }
        }
        
        private string GetSaveFilePath(string saveName)
        {
            string safeName = SanitizeFileName();
            return Path.Combine(_saveDir, $"{safeName}{SAVE_FILE_EXT}");
            
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