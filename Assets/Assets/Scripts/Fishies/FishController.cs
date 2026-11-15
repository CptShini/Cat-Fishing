using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Perception;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine;

namespace Assets.Scripts.Fishies
{
    [RequireComponent(typeof(FishNormalMovement))]
    [RequireComponent(typeof(FishCaughtMovement))]
    public class FishController : MonoBehaviour
    {
        public Fish Fish { get; private set; }
        
        private FishNormalMovement _normalMovement;
        private FishCaughtMovement _caughtMovement;

        [SerializeField] private FishVisuals visuals;
        [SerializeField] private FishSightHandler sightHandler;
        [SerializeField] private FishPerceptionHandler perceptionHandler;

        private void Awake()
        {
            sightHandler.OnSeePlayer.AddListener(Flee);
            perceptionHandler.OnDetectPlayer.AddListener(Flee);
            
            _normalMovement = GetComponent<FishNormalMovement>();
            _caughtMovement = GetComponent<FishCaughtMovement>();
            
            _normalMovement.MovementDone.AddListener(Wander);
            _caughtMovement.OnLose.AddListener(Flee);
        }

        public void Init(FishSpecies species)
        {
            Fish = new(species);

            name = $"{species.Name} ({Fish.Weight:F1} KG)";
            visuals.Init(species.Icon, Fish.RelativeWeight);
            
            _normalMovement.Init(species.Speed, species.Biome);
            _caughtMovement.Init(species.CatchPattern, 1f);
            
            sightHandler.Init(species.Reaction, species.Sight);
            perceptionHandler.Init(species.Reaction, species.Perception);
            
            Wander();
        }

        private void Wander()
        {
            ToggleSystems(false);
            _normalMovement.Wander();
        }
    
        private void Flee(Vector2 fleePoint)
        {
            ToggleSystems(false);
            _normalMovement.Flee(fleePoint);
        }

        public void Catch()
        {
            ToggleSystems(true);
            _caughtMovement.Catch();
        }

        private void ToggleSystems(bool gettingCaught)
        {
            _normalMovement.enabled = !gettingCaught;
            _caughtMovement.enabled = gettingCaught;
            sightHandler.enabled = !gettingCaught;
            perceptionHandler.enabled = !gettingCaught;
        }
    }
}