using Assets.Scripts.Fish.Movement;
using Assets.Scripts.Fish.Perception;
using UnityEngine;

namespace Assets.Scripts.Fish
{
    [RequireComponent(typeof(FishNormalMovement))]
    [RequireComponent(typeof(FishCaughtMovement))]
    public class FishController : MonoBehaviour
    {
        private FishNormalMovement _normalMovement;
        private FishCaughtMovement _caughtMovement;
        
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
        
            Wander();
        }

        private void Wander()
        {
            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Wander();
        }
    
        private void Flee(Vector2 fleePoint)
        {
            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Flee(fleePoint);
        }

        public void Catch()
        {
            _normalMovement.enabled = false;
            _caughtMovement.enabled = true;
            
            _caughtMovement.Catch();
        }
    }
}