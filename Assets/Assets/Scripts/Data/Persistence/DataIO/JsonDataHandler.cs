using System;
using System.IO;
using System.Linq;
using Assets.Scripts.Data.Domain;
using UnityEngine;

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

        private string GetSaveFilePath(string saveName) =>
            Path.Combine(_saveDir, $"{saveName}.json");

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
            string fullPath = GetSaveFilePath(saveName);
            if (!File.Exists(fullPath)) return null;
            
            try
            {
                using FileStream fs = new(fullPath, FileMode.Open);
                using StreamReader sr = new(fs);
                string dataToLoad = sr.ReadToEnd();

                #if !UNITY_EDITOR
                EncryptDecrypt(ref dataToLoad);
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
            SanitizeFileName(ref saveName);
            
            string fullPath = GetSaveFilePath(saveName);
            try
            {
                CreateSaveDirectoryIfNotPresent();
            
                string dataToSave = JsonUtility.ToJson(data, true);
                
                #if !UNITY_EDITOR
                EncryptDecrypt(ref dataToSave);
                #endif
                
                using FileStream fs = new(fullPath, FileMode.Create);
                using StreamWriter sw = new(fs);
                sw.Write(dataToSave);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving data to path \"{fullPath}\": {e.Message}");
            }
        }
        
        private void CreateSaveDirectoryIfNotPresent()
        {
            bool saveDirExists = Directory.Exists(_saveDir);
            if (!saveDirExists) Directory.CreateDirectory(_saveDir);
        }

        private static void SanitizeFileName(ref string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string newName =
                string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
            
            fileName = newName;
        }

        #if !UNITY_EDITOR
        private static void EncryptDecrypt(ref string data)
        {
            const string codeword = "KoiKat";
            
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char) (data[i] ^ codeword[i % codeword.Length]);
            }
            
            data = modifiedData;
        }
        #endif
    }
}