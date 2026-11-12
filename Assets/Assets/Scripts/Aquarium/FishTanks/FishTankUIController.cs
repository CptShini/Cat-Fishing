using Assets.Scripts.Data.Domain.Aquarium;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTankUIController : MonoBehaviour
    {
        public static FishTankUIController Instance { get; private set; }

        [SerializeField] [Min(0)] private int feedingPrice = 5;
        
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI 
            tankNameText,
            tankCapacityText,
            tankEntertainmentText;

        [SerializeField] private Button feedButton;
        [SerializeField] private ParticleSystem feedParticles;
        
        [Space]
        
        [SerializeField] private FishTankSpawner spawner;
        [SerializeField] private FishGrabber grabber;

        private FishTank _currentTank, _prevTank;

        public bool IsOpen { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
            grabber.DropFish.AddListener(OnFishDropped);
            feedButton.interactable = GoldManager.Instance.HasEnoughGold(feedingPrice);
        }

        public void Open(FishTank tank)
        {
            if (IsOpen) return;
            
            _currentTank = tank;
            
            panel.SetActive(true);
            IsOpen = true;
            grabber.enabled = true;

            PauseController.IsGamePaused = true;

            if (!_prevTank || tank.Tank.Id != _prevTank.Tank.Id)
            {
                tankNameText.text = tank.Tank.Name;
                UpdateTankText();
                spawner.Clear();
            }
            
            foreach (TankFish fish in tank.Tank.Fishes)
            {
                spawner.SpawnFish(fish);
            }
        }

        public void AddFish(TankFish fish)
        {
            if (!IsOpen) return;
            
            _currentTank.AddFish(fish);
            spawner.SpawnFish(fish);
            UpdateTankText();
        }

        public void Feed()
        {
            if (!IsOpen || !GoldManager.Instance.SpendGold(feedingPrice)) return;
            
            _currentTank.Feed();
            feedParticles.Play();
            feedButton.interactable = GoldManager.Instance.HasEnoughGold(feedingPrice);
            UpdateTankText();
        }

        public void Close()
        {
            if (!IsOpen) return;
            
            _prevTank = _currentTank;
            _currentTank = null;
            
            panel.SetActive(false);
            IsOpen = false;
            grabber.enabled = false;
            feedParticles.Stop();
            feedParticles.Clear();

            PauseController.IsGamePaused = false;
        }

        private void OnFishDropped(TankFish fish, Vector2 dropPos)
        {
            if (!_currentTank || !spawner.OutsideBounds(dropPos)) return;

            _currentTank.RemoveFish(fish);
            spawner.RemoveFish(fish);
            FishCollectionManager.Instance.Add(fish);
            UpdateTankText();
        }

        private void UpdateTankText()
        {
            int count = _currentTank.Tank.Fishes.Count;
            int capacity = _currentTank.Tank.Capacity;
            tankCapacityText.text = $"{count}/{capacity}";
            tankCapacityText.color = 
                count <= capacity 
                    ? Color.black 
                    : Color.red;

            tankEntertainmentText.text = $"Entertainment: {_currentTank.Tank.Entertainment:F2}";
        }
    }
}