using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
                GameObject saveFileButton = Instantiate(saveFileButtonPrefab, saveFileButtonContent);
                saveFileButton.GetComponentInChildren<TextMeshProUGUI>().text = saveFile.ToString();
                saveFileButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioManager.Instance.Play(AudioId.OpeningDoor);
                    SaveManager.Instance.Select(saveFile.Name);
                    SaveManager.Instance.Load();
                    GameManager.Instance.ChangeScene(1);
                });
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
            SaveManager.Instance.Select(_newSaveName);
            SaveManager.Instance.StartNew();
            GameManager.Instance.ChangeScene(1);
        }
    }
}