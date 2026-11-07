using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    public class AquariumFish : MonoBehaviour
    {
        public Fish Fish { get; private set; }
        
        private FishNormalMovement _movement;
        
        [SerializeField] private FishVisuals visuals;

        private void Awake()
        {
            _movement = GetComponent<FishNormalMovement>();
            _movement.MovementDone.AddListener(_movement.Wander);
        }

        public void Init(Fish fish)
        {
            name = fish.Species.Name;
            
            Fish = fish;
            visuals.Init(fish.Species.Icon, Fish.RelativeWeight);
            _movement.Init(fish.Species.Speed);
            
            _movement.Wander();
        }

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;
    }
}