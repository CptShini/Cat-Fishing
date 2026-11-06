using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    public class AquariumFish : MonoBehaviour
    {
        [SerializeField] private FishSpecies speciesFuck;
        
        public Fish Fish { get; private set; }
        
        private FishNormalMovement _movement;
        
        [SerializeField] private FishVisuals visuals;

        private void Start()
        {
            _movement = GetComponent<FishNormalMovement>();
            _movement.MovementDone.AddListener(_movement.Wander);
            
            Init(new(speciesFuck));
            _movement.Wander();
        }

        public void Init(Fish fish)
        {
            Fish = fish;
            visuals.Init(fish.Species.Icon, Fish.RelativeWeight);
            _movement.Init(fish.Species.Speed);
        }
    }
}