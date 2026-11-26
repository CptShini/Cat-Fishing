using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
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
            foreach (SaveFile saveFile in SaveManager.Instance.Saves)
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
            SFXManager.Instance.Play(SFXId.InventoryFull); //different sound? paper?
            SaveManager.Instance.StartNew(_newSaveName);
            GameManager.Instance.ChangeScene(Scenes.Letter);
        }
    }
}