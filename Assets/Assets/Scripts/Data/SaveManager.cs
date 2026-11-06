using System.Linq;
using System.Text;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [RequireComponent(typeof(DataPersistenceManager))]
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
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
        
        // TODO: Don't just load most recent save lol
        private void Start()
        {
            SaveFile[] saves = GetSaves();
            { // For literally no good reason
                StringBuilder sb = new();
                sb.Append($"{saves.Length} save");
                if (saves.Length != 1) sb.Append('s');
                sb.Append(" found");
                if (saves.Length != 0)
                {
                    sb.Append(":\n");
                    saves.ToList().ForEach(save => sb.AppendLine(save.ToString()));
                }
                
                print(sb.ToString());
            }

            Select(saves.Length > 0 ? saves[0].Name : "default");
            Load();
        }
        
        public void Select(string saveName) => _currentSaveName = saveName;
        
        public bool NoSaveSelected()
        {
            bool invalidName = string.IsNullOrWhiteSpace(_currentSaveName);
            if (invalidName) Debug.LogWarning("No save selected!");
            
            return invalidName;
        }
        
        public SaveFile[] GetSaves() => _dataPersistenceManager.GetSaves();

        public void StartNew() => _dataPersistenceManager.NewGame();

        public void Load()
        {
            if (NoSaveSelected())
                return;
            
            _dataPersistenceManager.LoadGame(_currentSaveName);
        }

        public void Save()
        {
            if (NoSaveSelected())
                return;
            
            _dataPersistenceManager.SaveGame(_currentSaveName);
        }

        private void OnDestroy() => Save();
    }
}