using System.Collections.Generic;
using Assets.Scripts.Fish.Navigation;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(FishMover))]
    [RequireComponent(typeof(FishNavigator))]
    public class FishNormalMovement : MonoBehaviour
    {
        private FishSpeed _speedState;
        
        private FishMover _mover;
        private FishNavigator _navigator;

        private void Awake()
        {
            _mover = GetComponent<FishMover>();
            _navigator = GetComponent<FishNavigator>();

            _navigator.DestinationReached.AddListener(Wander);
        }

        private void Update() => _mover.DesiredDirection = _navigator.CurrentDirection;

        public void Wander()
        {
            _navigator.Wander();
            
            _speedState = GetRandomWanderSpeed();
            _mover.SetSpeed(_speedState);

            return;
            
            static FishSpeed GetRandomWanderSpeed()
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
        }

        public void Flee(Vector2 fleePoint)
        {
            _navigator.Flee(fleePoint);
            
            _speedState = FishSpeed.Max;
            _mover.SetSpeed(_speedState);
        }
    }
}