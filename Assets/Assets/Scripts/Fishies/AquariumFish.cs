using Assets.Scripts.Data.Domain.Aquarium;
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
            name = fish.Fish.Species.Name;
            
            Fish = fish;
            visuals.Init(fish.Fish.Species.Icon, Fish.Fish.RelativeWeight);
            _movement.Init(fish.Fish.Species.Speed);
            
            _movement.Wander();
        }

        private void LateUpdate()
        {
            if (happinessText.gameObject.activeSelf != _hovering)
            {
                happinessText.gameObject.SetActive(_hovering);
                happinessText.text = $"{Fish.Happiness}/10";
            }

            _hovering = false;
        }

        public void Hovering() => _hovering = true;

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;
    }
}