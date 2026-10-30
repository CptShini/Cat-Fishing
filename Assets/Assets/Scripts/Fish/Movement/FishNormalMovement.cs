using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fish.Navigation;
using Assets.Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(FishMover))]
    public class FishNormalMovement : MonoBehaviour
    {
        private FishSpeed _speedState;
        
        private FishMover _mover;
        private FishNavigator _navigator;

        private void Awake() => _mover = GetComponent<FishMover>();

        public void Wander()
        {
            _mover.DesiredDirection = Random.onUnitCircle;
            
            _speedState = GetRandomWanderSpeed();
            _mover.SetSpeed(_speedState);
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

        public void Flee(Vector2 fleePoint)
        {
            _mover.DesiredDirection = (Vector2)transform.position - fleePoint;
            
            _speedState = FishSpeed.Max;
            _mover.SetSpeed(_speedState);
        }
    }
}