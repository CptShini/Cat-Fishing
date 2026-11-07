using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence.DataIO;
using UnityEngine;

namespace Assets.Scripts.Data.Persistence
{
    public class DataPersistenceManager : MonoBehaviour
    {
        private GameData _gameData;
        private List<IDataPersistence> _dataPersistenceObjects;
        
        private DataIOHandler _dataIOHandler;
        
        private void Awake()
        {
            _dataIOHandler = GetComponent<DataIOHandler>();
            if (_dataIOHandler == null)
                throw new MissingComponentException("No data IO handler attached.");
        }
        
        public SaveFile[] GetSaves() => _dataIOHandler.GetSaves();

        public void NewGameData() => _gameData = new();
        public void LoadFromSaveFile(string saveName) => _gameData = _dataIOHandler.Load(saveName) ?? new();
        public void SaveToSaveFile(string saveName) => _dataIOHandler.Save(_gameData, saveName);
        public void LoadDataToGame() => _dataPersistenceObjects.ForEach(dpo => dpo.LoadData(_gameData));
        public void SaveGameToData() => _dataPersistenceObjects.ForEach(dpo => dpo.SaveData(ref _gameData));

        public void UpdateDataPersistenceObjects() =>
            _dataPersistenceObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IDataPersistence>()
                    .ToList();
    }
}