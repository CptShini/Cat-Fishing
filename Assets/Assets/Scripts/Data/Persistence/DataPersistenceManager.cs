using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence.DataIO;
using UnityEngine;

namespace Assets.Scripts.Data.Persistence
{
    public class DataPersistenceManager : MonoBehaviour
    {
        private GameData _gameData = new();
        private List<IDataPersistence> _dataPersistenceObjects;
        
        private DataIOHandler _dataIOHandler;
        
        private void Awake()
        {
            _dataIOHandler = GetComponent<DataIOHandler>();
            if (_dataIOHandler == null)
                throw new MissingComponentException("No data IO handler attached.");
        }
        
        public SaveFile[] GetSaves() => _dataIOHandler.GetSaves();

        public SaveFile StartNewSaveFile(string saveName)
        {
            _gameData = new();
            return _dataIOHandler.Create(saveName);
        }
        public void LoadFromSaveFile(SaveFile saveFile) => _gameData = _dataIOHandler.Load(saveFile) ?? new();
        public void SaveToSaveFile(SaveFile saveFile) => _dataIOHandler.Save(_gameData, saveFile);
        public void DeleteSaveFile(SaveFile saveFile) => _dataIOHandler.Delete(saveFile);
        
        public void LoadDataToGame() => _dataPersistenceObjects.ForEach(dpo => dpo.LoadData(_gameData));
        public void SaveGameToData() => _dataPersistenceObjects.ForEach(dpo => dpo.SaveData(ref _gameData));
        public void FindDPOs() =>
            _dataPersistenceObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IDataPersistence>()
                    .ToList();
    }
}