using Assets.Scripts.Data;
using Assets.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private int aquariumSceneIndex = 1;
        [SerializeField] private TextMeshProUGUI playButtonText;

        private void Start()
        {
            playButtonText.text = SaveManager.Instance.HasSave 
                ? "Continue" 
                : "New Game";
        }

        public void Play()
        {
            if (!SaveManager.Instance.HasSave) 
                SaveManager.Instance.Select("Default");

            GameManager.Instance.ChangeScene(aquariumSceneIndex);
        }

        public void Quit() => Application.Quit();
    }
}
