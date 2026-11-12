using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    public class AquariumFish : MonoBehaviour
    {
        public TankFish Fish { get; private set; }
        
        private FishNormalMovement _movement;
        
        [SerializeField] private FishVisuals visuals;

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

        public void Grab() => _movement.enabled = false;
        public void Release() => _movement.enabled = true;
    }
}