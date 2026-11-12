using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Management;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playButtonText;
        [SerializeField] private TextMeshProUGUI continueButtonText;

        private void Start()
        {
            Initialize();
            SaveManager.Instance.CurrentSaveChanged.AddListener(Initialize);
        }

        private void Initialize()
        {
            SaveFile currentSave = SaveManager.Instance.CurrentSave;
            if (currentSave == null)
            {
                playButtonText.text = "New Game";
                continueButtonText.text = "";
                continueButtonText.gameObject.SetActive(false);
            }
            else
            {
                playButtonText.text = "Continue";
                continueButtonText.text = currentSave.ToString();
                continueButtonText.gameObject.SetActive(true);
            }
        }

        public void Play()
        {
            if (SaveManager.Instance.CurrentSave == null)
                SaveManager.Instance.StartNew("Default");

            AudioManager.Instance.Play(AudioId.OpeningDoor);
            GameManager.Instance.ChangeScene(Scenes.Aquarium, "...");
        }
        
        public void ButtonClickSound() => AudioManager.Instance.Play(AudioId.ButtonClick);

        public void Quit() => Application.Quit();
    }
}
