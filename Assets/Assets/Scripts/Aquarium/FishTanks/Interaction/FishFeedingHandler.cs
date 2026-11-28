using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    public class FishFeedingHandler : MonoBehaviour
    {
        public readonly UnityEvent<Vector2> FeedFish = new();
        
        private AquariumCursorMapper _cursorMapper;

        private void Awake() => _cursorMapper = GetComponent<AquariumCursorMapper>();

        public void TryFeed(Vector2 mousePosition)
        {
            if (!_cursorMapper.TryGetWorldPosition(mousePosition, out Vector2 cursorWorldPosition))
                return;
            
            FeedFish.Invoke(cursorWorldPosition);
        }
    }
}