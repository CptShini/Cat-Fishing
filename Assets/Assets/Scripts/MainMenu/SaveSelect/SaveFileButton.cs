using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu.SaveSelect
{
    public class SaveFileButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI saveFileText;
        [SerializeField] private Button playSaveButton, deleteSaveButton;

        public void Init(SaveFile saveFile)
        {
            saveFileText.text = saveFile.ToString();
            playSaveButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.Play(AudioId.OpeningDoor);
                SaveManager.Instance.Select(saveFile);
                SaveManager.Instance.Load();
                GameManager.Instance.ChangeScene(Scenes.Aquarium);
            });
            deleteSaveButton.onClick.AddListener(() =>
            {
                SaveManager.Instance.Delete(saveFile);
                Destroy(gameObject);
            });
        }
    }
}