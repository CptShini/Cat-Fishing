using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Aquarium.FishTanks.CleaningMiniGame;
using Assets.Scripts.Aquarium.FishTanks.Interaction;
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
            tankEntertainmentText,
            upgradePriceText,
            feedingPriceText;

        [SerializeField] private SpriteRenderer tankDirtyImage;
        [SerializeField] private Button upgradeButton;

        [SerializeField] private ParticleSystem feedParticles;

        [Space]
        
        [SerializeField] private FishTankSpawner spawner;
        [SerializeField] private FishInteractionController interactionController;

        [Space]

        [SerializeField] private SpriteEraser spriteEraser;
        [SerializeField, Range(0f, 100f)] private float requiredCleanPercentage = 90f;

        [Space]

        [SerializeField, Min(0.1f)]private float fullCleanFadeDuration = 1.5f;
        
        private bool _isFadingOut;
        private float _fadeTimer;
        private float _fadeStartDirtiness;

        private FishTank _currentTank, _prevTank;

        public bool IsOpen { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
            interactionController.DropFish.AddListener(OnFishDropped);
            interactionController.FeedFish.AddListener(OnFishFed);
            interactionController.CleanAtPosition.AddListener(OnCleanAtPosition);

            GoldManager.Instance.GoldChanged.AddListener(UpdateUpgradeUI);

            upgradeButton.onClick.AddListener(UpgradeCurrentTank);

            UpdateFeedingPriceUI();
        }

        public void Open(FishTank tank)
        {
            if (IsOpen)
                return;

            _currentTank = tank;
            panel.SetActive(true);

            IsOpen = true;
            interactionController.enabled = true;

            spriteEraser.ResetTexture();

            UpdateUpgradeUI();
            UpdateDirtinessVisual();

            if (!_prevTank || tank.Tank.Id != _prevTank.Tank.Id)
            {
                tankNameText.text = tank.Tank.Name;
                UpdateTankText();

                spawner.Clear();
                feedParticles.Stop();
                feedParticles.Clear();
            }

            foreach (TankFish fish in tank.Tank.Fishes)
                spawner.SpawnFish(fish);

            PauseController.IsGamePaused = true;
        }

        public void AddFish(TankFish fish, Vector2 mouseScreenPos)
        {
            if (!IsOpen) return;
            
            _currentTank.Tank.AddFish(fish);
            interactionController.CursorMapper.TryGetWorldPosition(mouseScreenPos, out Vector2 mouseWorldPos);
            spawner.SpawnFish(fish, mouseWorldPos);
            UpdateTankText();
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            _isFadingOut = false;

            _prevTank = _currentTank;
            _currentTank = null;

            panel.SetActive(false);
            IsOpen = false;
            interactionController.enabled = false;

            PauseController.IsGamePaused = false;
        }

        private void OnFishDropped(TankFish fish, Vector2 dropPos)
        {
            if (!_currentTank || !spawner.OutsideBounds(dropPos)) return;

            _currentTank.Tank.RemoveFish(fish);
            spawner.RemoveFish(fish);
            FishCollectionManager.Instance.Add(fish);
            UpdateTankText();
        }
        
        private void OnFishFed(TankFish fish, Vector2 cursorWorldPos)
        {
            if (!IsOpen || !GoldManager.Instance.SpendGold(feedingPrice)) return;
            
            feedParticles.transform.position = cursorWorldPos;
            feedParticles.Play();
            if (fish == null) return;
            
            fish.Feed();
            UpdateTankText();
        }

        private void UpdateFeedingPriceUI() => 
            feedingPriceText.text = feedingPrice.ToString();

        private void UpdateTankText()
        {
            int count = _currentTank.Tank.Fishes.Count;
            int capacity = _currentTank.Tank.Capacity;
            tankCapacityText.text = $"{count}/{capacity}";
            tankCapacityText.color =
                count == 1
                ? Color.blue
                : count <= capacity 
                    ? Color.black 
                    : Color.red;

            UpdateEntertainmentText();
        }
        
        private void UpdateEntertainmentText() =>
            tankEntertainmentText.text = $"Entertainment: {_currentTank.Tank.Entertainment:F2}";
        
        private void UpdateUpgradeUI()
        {
            if (_currentTank == null)
            {
                upgradePriceText.text = "";
                upgradeButton.interactable = false;
                return;
            }

            var tank = _currentTank.Tank;
            var definition = _currentTank.Definition;

            int price = definition.GetUpgradePrice(tank.UpgradeLevel);
            if (price < 0)
            {
                upgradeButton.gameObject.SetActive(false);
                return;
            }

            upgradeButton.gameObject.SetActive(true);

            upgradePriceText.text = price.ToString();
            upgradeButton.interactable = GoldManager.Instance.HasEnoughGold(price);
        }

        private void UpgradeCurrentTank()
        {
            if (!_currentTank)
                return;

            Tank tank = _currentTank.Tank;
            FishTankDefinitionSO definition = _currentTank.Definition;

            int price = definition.GetUpgradePrice(tank.UpgradeLevel);
            if (price < 0 || !GoldManager.Instance.SpendGold(price))
                return;

            int newCapacity = definition.GetCapacity(tank.UpgradeLevel + 1);
            if (!tank.Upgrade(newCapacity))
                return;

            UpdateTankText();
            UpdateUpgradeUI();
        }

        // cleaning progress is based only on number of fully erased pixels and not time now
        private void OnCleanAtPosition(Vector2 worldPosition)
        {
            if (!_currentTank || !spriteEraser || _isFadingOut)
                return;

            if (!spriteEraser.EraseAt(worldPosition))
                return;

            float cleanPercentage = spriteEraser.GetErasedPercentage();

            if (cleanPercentage < requiredCleanPercentage)
                return;

            _isFadingOut = true;
            _fadeTimer = 0f;
            _fadeStartDirtiness = _currentTank.Tank.Dirtiness;

            Debug.Log($"{_currentTank.Tank.Name} is fully cleaned!");
        }

        private void UpdateDirtinessVisual()
        {
            if (!_currentTank) return;
            
            float dirtinessFactor = _currentTank.Tank.DirtinessFactor;
            float alpha = dirtinessFactor > 0f ? dirtinessFactor * 0.9f : 0f;
            tankDirtyImage.color = new(0f, 1f, 0f, alpha);

            _currentTank.UpdateDirtinessWarning();
            UpdateEntertainmentText();
        }

        private void Update()
        {
            if (!_isFadingOut || !_currentTank)
                return;

            _fadeTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(_fadeTimer / fullCleanFadeDuration);
            float newDirtiness = Mathf.Lerp(_fadeStartDirtiness, 0f, progress);
            float reduction = _currentTank.Tank.Dirtiness - newDirtiness;

            _currentTank.Tank.Dirtiness -= reduction;

            UpdateDirtinessVisual();

            if (progress < 1f) return;
            
            _currentTank.Tank.Dirtiness = 0f;

            _isFadingOut = false;
            UpdateDirtinessVisual();
        }
    }
}