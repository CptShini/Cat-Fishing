using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fish.Movement;
using Assets.Scripts.Fish.Perception;
using Assets.Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish
{
    [RequireComponent(typeof(FishMover))]
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
        private FishSpeed _speedState;

        private float _stateClock;

        private void Start()
        {
            sightHandler.OnSeePlayer.AddListener(Flee);
            perceptionHandler.onDetectPlayer.AddListener(Flee);
            
            _normalMovement = GetComponent<FishNormalMovement>();
            _caughtMovement = GetComponent<FishCaughtMovement>();

            _caughtMovement.onLoose.AddListener(Flee);
        
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
            _speedState = GetRandomWanderSpeed();
            _stateClock = Random.Range(minWanderTime, maxWanderTime);

            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Wander(_speedState);
        }

        private static FishSpeed GetRandomWanderSpeed()
        {
            Dictionary<FishSpeed, int> speedWeights = new()
            {
                { FishSpeed.Chill,  1 },
                { FishSpeed.Slow,   2 },
                { FishSpeed.Normal, 3 },
                { FishSpeed.Fast,   1 }
            };
            return RandomHelper.WeightedRandom(speedWeights);
        }

    
        private void Flee(Vector2 fleePoint, bool loose)
        {
            Debug.Log("FLEE GODDAMMIT");
            if(_state == FishState.Caught && !loose)
            {
                return;
            }
            _state = FishState.Flee;
            _speedState = FishSpeed.Max;
            _stateClock = Random.Range(minFleeTime, maxFleeTime);
        
            _normalMovement.enabled = true;
            _caughtMovement.enabled = false;
            
            _normalMovement.Flee(_speedState, fleePoint);
        }

        public void Catch()
        {
            _state = FishState.Caught;
            _speedState = FishSpeed.Max;
        
            _normalMovement.enabled = false;
            _caughtMovement.enabled = true;
            
            _caughtMovement.Catch(_speedState);
        }

        public void Caught()
        {
            _caughtMovement.Caught();
        }
    }
}