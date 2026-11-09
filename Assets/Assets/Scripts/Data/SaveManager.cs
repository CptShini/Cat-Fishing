using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [RequireComponent(typeof(DataPersistenceManager))]
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
        public bool HasSave { get; private set; }
        public SaveFile[] Saves { get; private set; }
        
        private bool _isInitialized;
        private string _currentSaveName;
        private DataPersistenceManager _dataPersistenceManager;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _dataPersistenceManager = GetComponent<DataPersistenceManager>();
        }

        public void Initialize()
        {
            if (_isInitialized) return;
            
            Saves = _dataPersistenceManager.GetSaves();
            
            HasSave = Saves.Length > 0;
            if (HasSave)
            {
                Select(Saves[0].Name);
                Load();
            }
            else
            {
                StartNew();
            }
            
            _isInitialized = true;
        }

        public void Select(string saveName) => _currentSaveName = saveName;
        
        public bool NoSaveSelected()
        {
            bool invalidName = string.IsNullOrWhiteSpace(_currentSaveName);
            if (invalidName) Debug.LogWarning("No save selected!");
            
            return invalidName;
        }

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

        public void FindDPOs() => _dataPersistenceManager.FindDataPersistenceObjects();

        private void OnApplicationQuit()
        {
            SaveData();
            Save();
        }
    }
}