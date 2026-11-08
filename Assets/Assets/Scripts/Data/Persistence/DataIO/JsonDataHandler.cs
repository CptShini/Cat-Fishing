using System;
using System.IO;
using System.Linq;
using Assets.Scripts.Data.Domain;
using UnityEngine;

#if !UNITY_EDITOR
using System.Security.Cryptography;
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
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Save file \"{saveName}\" does not exist.");
                return null;
            }
            
            try
            {
                using FileStream fs = new(fullPath, FileMode.Open);
                using StreamReader sr = new(fs);
                string dataToLoad = sr.ReadToEnd();

                #if !UNITY_EDITOR
                Decrypt(ref dataToLoad);
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
                Encrypt(ref dataToSave);
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
        private static readonly byte[] Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("KoiKat"));

        private static void Encrypt(ref string data)
        {
            byte[] nonce = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }

            byte[] plain = Encoding.UTF8.GetBytes(data);
            byte[] encrypted = new byte[plain.Length];
            byte[] tag = new byte[16];

            using (var aes = new AesGcm(Key))
            {
                aes.Encrypt(nonce, plain, encrypted, tag);
            }

            byte[] result = new byte[nonce.Length + tag.Length + encrypted.Length];

            Buffer.BlockCopy(nonce, 0, result, 0, 12);
            Buffer.BlockCopy(tag, 0, result, 12, 16);
            Buffer.BlockCopy(encrypted, 0, result, 28, encrypted.Length);

            data = Convert.ToBase64String(result);
        }

        private static void Decrypt(ref string data)
        {
            byte[] input = Convert.FromBase64String(data);

            if (input.Length < 28)
                throw new CryptographicException("Invalid encrypted data.");

            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] encrypted = new byte[input.Length - 28];
            byte[] plain = new byte[encrypted.Length];

            Buffer.BlockCopy(input, 0, nonce, 0, 12);
            Buffer.BlockCopy(input, 12, tag, 0, 16);
            Buffer.BlockCopy(input, 28, encrypted, 0, encrypted.Length);

            using (var aes = new AesGcm(Key))
            {
                aes.Decrypt(nonce, encrypted, tag, plain);
            }

            data = Encoding.UTF8.GetString(plain);
        }
        #endif
    }
}