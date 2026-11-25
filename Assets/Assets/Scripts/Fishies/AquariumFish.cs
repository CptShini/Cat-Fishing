using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    public class AquariumFish : MonoBehaviour
    {
        public TankFish Fish { get; private set; }

        private bool _hovering;
        private FishNormalMovement _movement;
        
        [SerializeField] private FishVisuals visuals;
        [SerializeField] private Image happinessImage;

        [Header("Happiness Sprites")]
        [SerializeField] private Sprite sadSprite;
        [SerializeField] private Sprite neutralSprite;
        [SerializeField] private Sprite happySprite;

        [Header("Happiness Thresholds")]
        [SerializeField, Range(0, 10)] private int sadMax = 4;
        [SerializeField, Range(0, 10)] private int neutralMax = 7;

        private void Awake()
        {
            _movement = GetComponent<FishNormalMovement>();
            _movement.MovementDone.AddListener(_movement.Wander);
        }

        public void Init(TankFish fish)
        {
            Fish = fish;
            
            name = $"{fish.Fish.Species.Name} ({fish.Fish.Weight:F1} KG)";
            visuals.Init(fish.Fish.Species.Icon, fish.Fish.Size);
            
            _movement.Init(fish.Fish.Species.Speed, fish.Fish.Size);
            
            _movement.Wander();
            Fish.Fed.AddListener(UpdateHappinessImage);
            UpdateHappinessImage();
        }

        private void LateUpdate()
        {
            if (happinessImage.gameObject.activeSelf != _hovering)
            {
                happinessImage.gameObject.SetActive(_hovering);
                UpdateHappinessImage();
            }

            _hovering = false;
        }
        
        private void UpdateHappinessImage()
        {
            int happiness = Fish.Happiness;

            if (happiness <= sadMax)
                happinessImage.sprite = sadSprite;
            else if (happiness <= neutralMax)
                happinessImage.sprite = neutralSprite;
            else
                happinessImage.sprite = happySprite;
        }

        public void Hovering() => _hovering = true;

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;
    }
}