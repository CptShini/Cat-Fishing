
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseControllerManager : MonoBehaviour
{
    public static PauseControllerManager Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject soundOptionsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject tutorialMenu;
    public bool isPaused { get; private set; }
    private int sceneIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }


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
        if (optionsMenu.activeSelf)
            ResumeGame();

        return;
    }

    // Gameplay scenes
    if (!isPaused)
    {
        PauseGame();
        return;
    }

    if (optionsMenu.activeSelf)
        ShowPauseMenu();
    else if (pausePanel.activeSelf)
        ResumeGame();
}


    // Temp fix - disable SFX when game is paused
    private void LateUpdate()
    {
        if (isPaused)
        {
            var sfxObjects = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var sfx in sfxObjects)
            {
                if (sfx.name.Contains("(SFX)") || sfx.name.Contains("(Loop)"))
                    sfx.Pause();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);

        if (sceneIndex == 0)
            ShowOptions();
        else
            ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        HideAllMenus();
        pauseMenu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        HideAllMenus();
        Debug.Log("Showing Pause Menu");
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

    private void HideAllMenus()
    {
        pausePanel.SetActive(false);
        optionsMenu.SetActive(false);
        soundOptionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
