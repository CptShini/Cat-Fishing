using Assets.Scripts.Data.Persistence;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Data
{
    [RequireComponent(typeof(DataPersistenceManager))]
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
        public SaveFile CurrentSave { get; private set; }
        public SaveFile[] Saves { get; private set; }
        
        public readonly UnityEvent CurrentSaveChanged = new();
        
        private bool _isInitialized, _saveListDirty;
        private DataPersistenceManager _dpm;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _dpm = GetComponent<DataPersistenceManager>();
        }

        public void Initialize()
        {
            if (_isInitialized) return;
            
            Saves = _dpm.GetSaves();
            if (Saves.Length > 0)
            {
                Select(Saves[0]);
                Load();
            }
            
            _isInitialized = true;
        }

        public void RefetchSaves()
        {
            if (!_saveListDirty) return;
            
            Saves = _dpm.GetSaves();
            
            _saveListDirty = false;
        }
        
        public void Select(SaveFile saveFile)
        {
            CurrentSave = saveFile;
            CurrentSaveChanged.Invoke();
        }

        private bool NoSaveSelected()
        {
            bool noCurrentSave = CurrentSave == null;
            if (noCurrentSave) Debug.LogWarning("No save selected!");
            
            return noCurrentSave;
        }

        public void StartNew(string saveName)
        {
            SaveFile newSaveFile = _dpm.StartNewSaveFile(saveName);
            Select(newSaveFile);
            
            _saveListDirty = true;
        }

        public void Load()
        {
            if (NoSaveSelected())
                return;
            
            _dpm.LoadFromSaveFile(CurrentSave);
        }

        public void Save()
        {
            if (NoSaveSelected())
                return;
            
            _dpm.SaveToSaveFile(CurrentSave);
        }

        public void Delete(SaveFile saveFile)
        {
            _dpm.DeleteSaveFile(saveFile);
            _saveListDirty = true;
            
            if (saveFile == CurrentSave) Select(null);
        }

        private void OnApplicationQuit()
        {
            _dpm.SaveGameToData();
            Save();
        }
    }
}