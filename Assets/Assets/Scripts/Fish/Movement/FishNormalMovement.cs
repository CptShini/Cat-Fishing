using System.Collections.Generic;
using Assets.Scripts.Fish.Navigation;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(FishMover))]
    [RequireComponent(typeof(FishNavigator))]
    public class FishNormalMovement : MonoBehaviour
    {
        public readonly UnityEvent MovementDone = new();
        
        private FishMover _mover;
        private FishNavigator _navigator;

        private void Awake()
        {
            _mover = GetComponent<FishMover>();
            _navigator = GetComponent<FishNavigator>();
            
            _navigator.DestinationReached.AddListener(MovementDone.Invoke);
        }

        private void Update() => _mover.DesiredDirection = _navigator.CurrentDirection;

        public void Wander()
        {
            _navigator.Wander();
            
            FishSpeed wanderSpeed = GetRandomWanderSpeed();
            _mover.SetSpeed(wanderSpeed);

            return;
            
            static FishSpeed GetRandomWanderSpeed()
            {
                Dictionary<FishSpeed, int> speedWeights = new()
                {
                    { FishSpeed.Chill,  1 },
                    { FishSpeed.Slow,   2 },
                    { FishSpeed.Normal, 3 },
                    { FishSpeed.Fast,   2 }
                };
                return RandomHelper.WeightedRandom(speedWeights);
            }
        }

        public void Flee(Vector2 fleePoint)
        {
            _navigator.Flee(fleePoint);
            
            _mover.SetSpeed(FishSpeed.Max);
        }

        private void OnEnable()
        {
            _mover.enabled = true;
            _navigator.enabled = true;
        }

        private void OnDisable()
        {
            _mover.enabled = false;
            _navigator.enabled = false;
        }
    }
}