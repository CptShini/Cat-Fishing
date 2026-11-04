using Assets.Scripts.Fish.Domain;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.Fish.Perception;
using UnityEngine;

namespace Assets.Scripts.Fish
{
    [RequireComponent(typeof(FishNormalMovement))]
    [RequireComponent(typeof(FishCaughtMovement))]
    public class FishController : MonoBehaviour
    {
        [SerializeField] private FishSpecies speciesFuck;
        
        public Domain.Fish Fish { get; private set; }
        
        private FishNormalMovement _normalMovement;
        private FishCaughtMovement _caughtMovement;

        [SerializeField] private FishSpriteSetter spriteSetter;
        [SerializeField] private FishSightHandler sightHandler;
        [SerializeField] private FishPerceptionHandler perceptionHandler;

        private void Start()
        {
            sightHandler.OnSeePlayer.AddListener(Flee);
            perceptionHandler.OnDetectPlayer.AddListener(Flee);
            
            _normalMovement = GetComponent<FishNormalMovement>();
            _caughtMovement = GetComponent<FishCaughtMovement>();
            
            _normalMovement.MovementDone.AddListener(Wander);
            _caughtMovement.OnLose.AddListener(Flee);
            
            Init(speciesFuck); // Temporary until it's done by spawner
            Wander();
        }

        public void Init(FishSpecies species)
        {
            Fish = new(species);
            spriteSetter.SetSprite(species.Icon);
            
            _normalMovement.Init();
            _caughtMovement.Init(species.CatchPattern, 1f);
            
            sightHandler.Init(species.Reaction, species.Sight);
            perceptionHandler.Init(species.Reaction, species.Perception);
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