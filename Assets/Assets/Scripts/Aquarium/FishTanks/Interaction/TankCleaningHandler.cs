using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    public class TankCleaningHandler : MonoBehaviour
    {
        public readonly UnityEvent<Vector2> OnCleanPosition = new();
        
        private AquariumCursorMapper _cursorMapper;

        private void Awake() => _cursorMapper = GetComponent<AquariumCursorMapper>();
        
        public void Tick(Vector2 mousePosition, bool held)
        {
            if (!held)
                return;

            if (!_cursorMapper.TryGetWorldPosition(mousePosition, out Vector2 worldPosition))
                return;

            OnCleanPosition.Invoke(worldPosition);
        }
    }
}