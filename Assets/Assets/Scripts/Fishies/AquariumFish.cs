using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI happinessText;

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
            Fish.Fed.AddListener(UpdateHappinessText);
        }

        private void LateUpdate()
        {
            if (happinessText.gameObject.activeSelf != _hovering)
            {
                happinessText.gameObject.SetActive(_hovering);
                UpdateHappinessText();
            }

            _hovering = false;
        }
        
        private void UpdateHappinessText() =>
            happinessText.text = $"{Fish.Happiness}/10";

        public void Hovering() => _hovering = true;

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;
    }
}