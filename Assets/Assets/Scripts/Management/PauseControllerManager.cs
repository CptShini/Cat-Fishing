using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Management
{
    public class PauseControllerManager : MonoBehaviour
    {
        public static PauseControllerManager Instance { get; private set; }
        
        [SerializeField] private GameObject 
            pauseMenu,
            pausePanel,
            optionsMenu,
            soundOptionsMenu,
            controlsMenu,
            tutorialMenu,
            feedbackMenu;
        
        public bool isPaused { get; private set; }
        
        private int sceneIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start() => sceneIndex = SceneManager.GetActiveScene().buildIndex;

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
                return;

            // Submenus: return to Options.
            if (soundOptionsMenu.activeSelf || controlsMenu.activeSelf || tutorialMenu.activeSelf)
            {
                ShowOptions();
                return;
            }

            // Main Menu
            if (sceneIndex == 0)
            {
                if (optionsMenu.activeSelf || feedbackMenu.activeSelf)
                    ResumeGame();

                return;
            }

            // Gameplay scenes
            if (!isPaused)
            {
                PauseGame();
                ShowPauseMenu();
                return;
            }

            if (optionsMenu.activeSelf || feedbackMenu.activeSelf)
                ShowPauseMenu();
            else if (pausePanel.activeSelf)
                ResumeGame();
        }

        // Temp fix - disable SFX when game is paused
        private void LateUpdate()
        {
            if (!isPaused) return;
        
            var sfxObjects = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var sfx in sfxObjects)
            {
                if (sfx.name.Contains("(SFX)") || sfx.name.Contains("(Loop)"))
                    sfx.Pause();
            }
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;

            pauseMenu.SetActive(true);
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;

            HideAllMenus();
            pauseMenu.SetActive(false);
        }

        public void PauseForTutorial()
        {
            isPaused = true;
            Time.timeScale = 0f;
        }

        public void ResumeAfterTutorial()
        {
            isPaused = false;
            Time.timeScale = 1f;
        }

        public void ShowPauseMenu()
        {
            HideAllMenus();
            pausePanel.SetActive(true);
        }

        public void ShowOptions()
        {
            HideAllMenus();
            optionsMenu.SetActive(true);
        }

        public void ShowSoundOptions()
        {
            HideAllMenus();
            soundOptionsMenu.SetActive(true);
        }

        public void ShowControls()
        {
            HideAllMenus();
            controlsMenu.SetActive(true);
        }

        public void ShowTutorial()
        {
            HideAllMenus();
            tutorialMenu.SetActive(true);
        }

        public void ShowFeedback()
        {
            HideAllMenus();
            feedbackMenu.SetActive(true);
        }

        private void HideAllMenus()
        {
            pausePanel.SetActive(false);
            optionsMenu.SetActive(false);
            soundOptionsMenu.SetActive(false);
            controlsMenu.SetActive(false);
            tutorialMenu.SetActive(false);
            feedbackMenu.SetActive(false);
        }
        
        public void BackToMenu()
        {
            Time.timeScale = 1f;
            HideAllMenus();
            GameManager.Instance.ChangeScene(Scenes.MainMenu, "Returning to main menu...");
        }
    }
}
