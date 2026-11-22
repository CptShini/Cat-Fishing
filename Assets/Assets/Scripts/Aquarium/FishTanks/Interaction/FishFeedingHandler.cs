using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Fishies;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    public class FishFeedingHandler : MonoBehaviour
    {
        [SerializeField] private float feedingDistanceThreshold = 1f;

        public readonly UnityEvent<TankFish, Vector2> FeedFish = new();
        
        private AquariumCursorMapper _cursorMapper;

        private void Awake() => _cursorMapper = GetComponent<AquariumCursorMapper>();

        public void TryFeed(Vector2 mousePosition)
        {
            if (!_cursorMapper.TryGetWorldPosition(mousePosition, out Vector2 cursorWorldPosition))
                return;

            AquariumFish closestFish = null;
            float closestDistance = float.PositiveInfinity;
            AquariumFish[] fishes = FindObjectsByType<AquariumFish>(FindObjectsSortMode.None);
            foreach (AquariumFish fish in fishes)
            {
                float distance = Vector2.Distance(
                    cursorWorldPosition,
                    fish.transform.position
                );

                if (distance > feedingDistanceThreshold || distance >= closestDistance)
                    continue;

                closestFish = fish;
                closestDistance = distance;
            }
            
            FeedFish.Invoke(closestFish?.Fish, cursorWorldPosition);
        }
    }
}