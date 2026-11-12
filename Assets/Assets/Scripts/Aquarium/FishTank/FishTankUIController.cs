using Assets.Scripts.Data.Domain.Aquarium;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishTankUIController : MonoBehaviour
    {
        public static FishTankUIController Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text tankNameText;

        [Space]
        
        [SerializeField] private FishTankSpawner spawner;
        [SerializeField] private FishGrabber grabber;

        private FishTank _currentTank, _prevTank;

        public bool IsOpen { get; private set; }

        private void Awake() => Instance = this;

        private void Start() => grabber.DropFish.AddListener(OnFishDropped);

        public void Open(FishTank tank)
        {
            if (IsOpen) return;
            
            _currentTank = tank;
            
            panel.SetActive(true);
            IsOpen = true;
            grabber.enabled = true;

            PauseController.IsGamePaused = true;

            if (!_prevTank || tank.Id != _prevTank.Id)
            {
                tankNameText.text = tank.Name;
                spawner.Clear();
            }
            
            foreach (TankFish fish in tank.Fish)
            {
                spawner.SpawnFish(fish);
            }
        }

        public void AddFish(TankFish fish)
        {
            if (!IsOpen) return;
            
            _currentTank.AddFish(fish);
            spawner.SpawnFish(fish);
        }

        public void Close()
        {
            if (!IsOpen) return;
            
            _prevTank = _currentTank;
            _currentTank = null;
            
            panel.SetActive(false);
            IsOpen = false;
            grabber.enabled = false;

            PauseController.IsGamePaused = false;
        }

        private void OnFishDropped(TankFish fish, Vector2 dropPos)
        {
            if (!_currentTank || !spawner.OutsideBounds(dropPos)) return;

            _currentTank.RemoveFish(fish);
            spawner.RemoveFish(fish);
            FishCollectionManager.Instance.Add(fish);
        }
    }
}