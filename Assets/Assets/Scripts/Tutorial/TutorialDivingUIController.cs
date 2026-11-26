using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Management;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialDivingUIController : MonoBehaviour, IDataPersistence
    {
        public static TutorialDivingUIController Instance { get; private set; }
    
        [SerializeField] private GameObject
            tutorialPopup,
            catchingTutorialPopup;
    
        private bool _hasReadTutorial;
        private bool _hasReadCatchingTutorial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Invoke(nameof(ShowTutorial), 1f);
        }
    
        public void LoadData(GameData data)
        {
            _hasReadTutorial = data.Tutorials.TryGetValue("Diving", out bool read) && read;
            _hasReadCatchingTutorial = data.Tutorials.TryGetValue("CatchingMinigame", out bool catchingRead) && catchingRead;
        }

        public void SaveData(ref GameData data)
        {
            data.Tutorials["Diving"] = _hasReadTutorial;
            data.Tutorials["CatchingMinigame"] = _hasReadCatchingTutorial;
        }

        private void ShowTutorial()
        {
            if (_hasReadTutorial) return;
        
            tutorialPopup.SetActive(true);
            PauseControllerManager.Instance.PauseForTutorial();
        }

        public void CloseTutorial()
        {
            tutorialPopup.SetActive(false);
            _hasReadTutorial = true;

            PauseControllerManager.Instance.ResumeAfterTutorial();
        }

        public bool ShowCatchingTutorial()
        {
            if (_hasReadCatchingTutorial)
                return false;

            catchingTutorialPopup.SetActive(true);
            return true;
        }

        public void CloseCatchingTutorial()
        {
            catchingTutorialPopup.SetActive(false);
            _hasReadCatchingTutorial = true;

            CatchingMinigame.Instance.StartMinigameAfterTutorial();
        }
    }
}