using System.Collections.Generic;
using Assets.Scripts.Fishies.Navigation;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fishies.Movement
{
    [RequireComponent(typeof(FishMover))]
    [RequireComponent(typeof(FishNavigator))]
    public class FishNormalMovement : MonoBehaviour
    {
        public readonly UnityEvent MovementDone = new();
        
        private FishMover _mover;
        private FishNavigator _navigator;

        private bool _wandering;
        private float _wanderSpeedClock;
        
        [SerializeField] private Range speedChangeTime;
        [SerializeField] private int
            speedWeightChill = 1,
            speedWeightSlow = 2,
            speedWeightNormal = 4,
            speedWeightFast = 3;

        private void Awake()
        {
            _mover = GetComponent<FishMover>();
            _navigator = GetComponent<FishNavigator>();
            
            _navigator.DestinationReached.AddListener(MovementDone.Invoke);
        }

        public void Init(int speedStat, string biome = "All")
        {
            _mover.Init(speedStat); // TODO: size and/or weight should probably also play a part in mobility
            _navigator.Init(biome);
        }

        private void Update()
        {
            _mover.DesiredDirection = _navigator.CurrentDirection;
            UpdateWanderSpeedRandomly();
        }

        public void Wander()
        {
            _wandering = true;
            _wanderSpeedClock = 0f;
            _navigator.Wander();
        }
        
        public void Flee(Vector2 fleePoint)
        {
            _wandering = false;
            _navigator.Flee(fleePoint);
            _mover.SetSpeed(FishSpeed.Max);
        }

        private void UpdateWanderSpeedRandomly()
        {
            if (!_wandering) return;
            
            _wanderSpeedClock -= Time.deltaTime;
            if (_wanderSpeedClock > 0f) return;

            _wanderSpeedClock = speedChangeTime.Random();
            
            FishSpeed wanderSpeed = GetRandomWanderSpeed();
            _mover.SetSpeed(wanderSpeed);

            return;
            
            FishSpeed GetRandomWanderSpeed()
            {
                Dictionary<FishSpeed, int> speedWeights = new()
                {
                    { FishSpeed.Chill,  speedWeightChill },
                    { FishSpeed.Slow,   speedWeightSlow },
                    { FishSpeed.Normal, speedWeightNormal },
                    { FishSpeed.Fast,   speedWeightFast }
                };
                return RandomHelper.WeightedRandom(speedWeights);
            }
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