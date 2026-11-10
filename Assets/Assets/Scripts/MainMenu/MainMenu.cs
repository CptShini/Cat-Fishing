using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Helpers;
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
            if (!SaveManager.Instance.HasSave)
            {
                playButtonText.text = "New Game";
                continueButtonText.text = "";
                continueButtonText.gameObject.SetActive(false);
            }
            else
            {
                playButtonText.text = "Continue";
                continueButtonText.text = SaveManager.Instance.Saves[0].ToString();
                continueButtonText.gameObject.SetActive(true);
            }
        }

        public void Play()
        {
            if (!SaveManager.Instance.HasSave) 
                SaveManager.Instance.Select("Default");

            AudioManager.Instance.Play(AudioId.OpeningDoor);
            GameManager.Instance.ChangeScene(1);
        }
        
        public void ButtonClickSound() => AudioManager.Instance.Play(AudioId.ButtonClick);

        public void Quit() => Application.Quit();
    }
}
