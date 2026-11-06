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
            _dataPersistenceObjects = 
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IDataPersistence>()
                .ToList();
            
            _dataIOHandler = GetComponent<DataIOHandler>();
            if (_dataIOHandler == null)
                throw new MissingComponentException("No data IO handler attached.");
        }
        
        public SaveFile[] GetSaves() => _dataIOHandler.GetSaves();

        public void NewGame() => _gameData = new();

        public void LoadGame(string saveName)
        {
            _gameData = _dataIOHandler.Load(saveName);
            if (_gameData == null)
            {
                Debug.LogWarning($"Save file \"{saveName}\" not found! Loading blank save...");
                NewGame();
            }
            
            foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(_gameData);
            }
        }

        public void SaveGame(string saveName)
        {
            foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
            {
                dataPersistenceObj.SaveData(ref _gameData);
            }
            
            _dataIOHandler.Save(_gameData, saveName);
        }
    }
}