using System;
using System.Collections.Generic;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.Fish.Perception;
using Assets.Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Controller
{
    [RequireComponent(typeof(FishNormalMovement))]
    [RequireComponent(typeof(FishCaughtMovement))]
    public class FishController : MonoBehaviour
    {
        // Settings
        
        [SerializeField] private float 
            minWanderTime = 1f,
            maxWanderTime = 15f,
            minFleeTime = 5f,
            maxFleeTime = 10f;
        
        // References
        
        private FishNormalMovement _normalMovement;
        private FishCaughtMovement _caughtMovement;
        
        [SerializeField] private FishSightHandler sightHandler;
        [SerializeField] private FishPerceptionHandler perceptionHandler;
        
        // Logic
        
        private FishState _state;

        private float _stateClock;

        private void Start()
        {
            sightHandler.OnSeePlayer.AddListener(Flee);
            perceptionHandler.OnDetectPlayer.AddListener(Flee);
            
            _normalMovement = GetComponent<FishNormalMovement>();
            _caughtMovement = GetComponent<FishCaughtMovement>();

            _caughtMovement.OnLose.AddListener(Flee);
        
            Wander();
        }

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            _stateClock -= Time.deltaTime;
            if (_stateClock > 0f) return;

            switch (_state)
            {
                case FishState.Wander:
                    Wander();
                    break;
                case FishState.Flee:
                    Wander();
                    break;
                case FishState.Caught:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Wander()
        {
            _state = FishState.Wander;
            _stateClock = Random.Range(minWanderTime, maxWanderTime);

            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Wander();
        }
    
        private void Flee(Vector2 fleePoint, bool loose)
        {
            if (_state == FishState.Caught && !loose)
                return;
            
            _state = FishState.Flee;
            _stateClock = Random.Range(minFleeTime, maxFleeTime);
        
            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Flee(fleePoint);
        }

        public void Catch()
        {
            _state = FishState.Caught;
            
            _normalMovement.enabled = false;
            _caughtMovement.enabled = true;
            
            _caughtMovement.Catch();
        }
    }
}