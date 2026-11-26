using System.Collections.Generic;
using System.Linq;
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

        public IReadOnlyList<SaveFile> Saves => _saves.AsReadOnly();
        private List<SaveFile> _saves;
        
        public readonly UnityEvent CurrentSaveChanged = new();
        
        private bool _isInitialized;
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
            
            _saves = _dpm.GetSaves().ToList();
            if (_saves.Count > 0)
            {
                Select(_saves.First());
                Load();
            }
            
            _isInitialized = true;
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
            _saves.Insert(0, newSaveFile);
            
            Select(newSaveFile);
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
            _saves.Remove(saveFile);

            if (saveFile == CurrentSave) Select(_saves.FirstOrDefault());
        }

        private void OnApplicationQuit()
        {
            _dpm.SaveGameToData();
            Save();
        }
    }
}