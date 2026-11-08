using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [RequireComponent(typeof(DataPersistenceManager))]
    public class SaveManager : MonoBehaviour
    {
        private string _currentSaveName = "default";
        private DataPersistenceManager _dataPersistenceManager;
        
        private void Awake() => _dataPersistenceManager = GetComponent<DataPersistenceManager>();
        
        public void Select(string saveName) => _currentSaveName = saveName;
        
        public bool NoSaveSelected()
        {
            bool invalidName = string.IsNullOrWhiteSpace(_currentSaveName);
            if (invalidName) Debug.LogWarning("No save selected!");
            
            return invalidName;
        }
        
        public SaveFile[] GetSaves() => _dataPersistenceManager.GetSaves();

        public void StartNew() => _dataPersistenceManager.NewGameData();
        public void LoadData() => _dataPersistenceManager.LoadDataToGame();
        public void SaveData() => _dataPersistenceManager.SaveGameToData();
        
        public void Load()
        {
            if (NoSaveSelected())
                return;
            
            _dataPersistenceManager.LoadFromSaveFile(_currentSaveName);
        }

        public void Save()
        {
            if (NoSaveSelected())
                return;
            
            _dataPersistenceManager.SaveToSaveFile(_currentSaveName);
        }

        public void RefreshDPOs() => _dataPersistenceManager.UpdateDataPersistenceObjects();

        private void OnApplicationQuit()
        {
            SaveData();
            Save();
        }
    }
}