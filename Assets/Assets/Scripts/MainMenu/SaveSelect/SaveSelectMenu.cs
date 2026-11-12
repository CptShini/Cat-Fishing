using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Helpers;
using Assets.Scripts.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu.SaveSelect
{
    public class SaveSelectMenu : MonoBehaviour
    {
        [SerializeField] private GameObject saveFileButtonPrefab;
        [SerializeField] private Transform saveFileButtonContent;
        
        [Space]
        
        [SerializeField] private Button newSaveButton;

        private string _newSaveName;

        private void Start()
        {
            SaveFile[] saveFiles = SaveManager.Instance.Saves;
            foreach (SaveFile saveFile in saveFiles)
            {
                GameObject saveFileButtonObject = Instantiate(saveFileButtonPrefab, saveFileButtonContent);
                SaveFileButton saveFileButton = saveFileButtonObject.GetComponent<SaveFileButton>();
                saveFileButton.Init(saveFile);
            }
        }

        public void OnNewSaveNameChanged(string newSaveName)
        {
            _newSaveName = newSaveName;
            newSaveButton.interactable = !string.IsNullOrWhiteSpace(newSaveName);
        }

        public void NewSave()
        {
            AudioManager.Instance.Play(AudioId.OpeningDoor);
            SaveManager.Instance.StartNew(_newSaveName);
            GameManager.Instance.ChangeScene(Scenes.Aquarium, "...");
        }
    }
}