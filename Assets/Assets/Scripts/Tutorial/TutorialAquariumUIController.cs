using System.Collections.Generic;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialUIController : MonoBehaviour, IDataPersistence
    {
        public static TutorialUIController Instance { get; private set; }

        [Header("Tutorial UI")]
        [SerializeField] private GameObject tutorialCanvas;

        [Header("Tutorial Popups")]
        [SerializeField] private GameObject
            workbenchTutorialPopup,
            computerShopTutorialPopup,
            computerUpgradeTutorialPopup,
            computerEncyclopediaTutorialPopup,
            fishTankTutorialPopup;


        private Dictionary<string, GameObject> _tutorialPopups;
        private Dictionary<string, bool> _tutorials = new();

        private string _currentTutorial;
        private GameObject _currentPopup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _tutorialPopups = new()
            {
                { "Workbench", workbenchTutorialPopup },
                { "ComputerShop", computerShopTutorialPopup },
                { "ComputerUpgrade", computerUpgradeTutorialPopup },
                { "ComputerEncyclopedia", computerEncyclopediaTutorialPopup },
                { "FishTank", fishTankTutorialPopup }
            };

            tutorialCanvas.SetActive(false);

            foreach (var popup in _tutorialPopups.Values)
            {
                popup.SetActive(false);
            }
        }

        public void ShowTutorial(string tutorialId)
        {
            if (_currentPopup != null)
                return;

            if (!_tutorialPopups.TryGetValue(tutorialId, out GameObject popup))
            {
                Debug.LogWarning($"Tutorial not found: {tutorialId}");
                return;
            }

            if (_tutorials.TryGetValue(tutorialId, out bool isRead) && isRead)
                return;

            _currentTutorial = tutorialId;
            _currentPopup = popup;

            tutorialCanvas.SetActive(true);
            _currentPopup.SetActive(true);
        }

        public void CloseTutorial()
        {
            if (_currentPopup == null)
                return;

            _tutorials[_currentTutorial] = true;

            _currentPopup.SetActive(false);
            tutorialCanvas.SetActive(false);

            _currentPopup = null;
            _currentTutorial = null;
        }

        public void LoadData(GameData data) => 
            _tutorials = data.Tutorials.ToDictionary();

        public void SaveData(ref GameData data) => 
            data.Tutorials = _tutorials.ToSerializableDictionary();
    }
}