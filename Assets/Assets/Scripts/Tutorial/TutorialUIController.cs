using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class TutorialUIController : MonoBehaviour, IDataPersistence
{
    public static TutorialUIController Instance { get; private set; }

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialCanvas;

    [Header("Tutorial Popups")]
    [SerializeField] private GameObject workbenchTutorialPopup;
    //[SerializeField] private GameObject computerTutorialPopup;
    //[SerializeField] private GameObject aquariumTutorialPopup;

    private Dictionary<string, GameObject> tutorialPopups;

    private Dictionary<string, bool> tutorials = new();

    private string currentTutorial;
    private GameObject currentPopup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tutorialPopups = new Dictionary<string, GameObject>
        {
            { "Workbench", workbenchTutorialPopup },
            //{ "Computer", computerTutorialPopup },
            //{ "Aquarium", aquariumTutorialPopup }
        };

        tutorialCanvas.SetActive(false);

        foreach (var popup in tutorialPopups.Values)
        {
            popup.SetActive(false);
        }
    }

    public void ShowTutorial(string tutorialId)
    {
        if (currentPopup != null)
            return;

        if (!tutorialPopups.TryGetValue(tutorialId, out GameObject popup))
        {
            Debug.LogWarning($"Tutorial not found: {tutorialId}");
            return;
        }

        if (tutorials.TryGetValue(tutorialId, out bool isRead) && isRead)
            return;

        currentTutorial = tutorialId;
        currentPopup = popup;

        tutorialCanvas.SetActive(true);
        currentPopup.SetActive(true);
    }

    public void CloseTutorial()
    {
        if (currentPopup == null)
            return;

        tutorials[currentTutorial] = true;

        currentPopup.SetActive(false);
        tutorialCanvas.SetActive(false);

        currentPopup = null;
        currentTutorial = null;

        SaveManager.Instance.GetComponent<DataPersistenceManager>().SaveGameToData();

        SaveManager.Instance.Save();
    }

    public void LoadData(GameData data)
    {
        tutorials = new Dictionary<string, bool>();

        foreach (var tutorial in data.Tutorials)
        {
            tutorials[tutorial.Key] = tutorial.Value;
        }
    }

    public void SaveData(ref GameData data)
    {
        foreach (var tutorial in tutorials)
        {
            data.Tutorials[tutorial.Key] = tutorial.Value;
        }
    }
}