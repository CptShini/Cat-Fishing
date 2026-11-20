using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Fishies;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    public class FishDragHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask draggableLayer;

        public readonly UnityEvent<TankFish, Vector2> OnDrop = new();
        
        private AquariumCursorMapper _cursorMapper;

        private AquariumFish _targetFish;
        private Vector2 _offset;
        private bool _dragging;

        private void Awake() => _cursorMapper = GetComponent<AquariumCursorMapper>();

        public void UpdateHover(Vector2 mousePosition)
        {
            if (!_cursorMapper.TryGetMouseRay(mousePosition, out var ray))
                return;

            AquariumFish hoveredFish = GetFishUnderMouse(ray);
            hoveredFish?.Hovering();
        }
        
        public void Tick(Vector2 mousePosition, bool pressed, bool held, bool released)
        {
            if (pressed && _cursorMapper.TryGetMouseRay(mousePosition, out var ray))
            {
                var hoveredFish = GetFishUnderMouse(ray);
                if (hoveredFish)
                    StartGrab(hoveredFish, mousePosition);
            }

            if (held)
                Drag(mousePosition);

            if (released)
                Drop();
        }

        private AquariumFish GetFishUnderMouse(Ray ray)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(
                ray,
                Mathf.Infinity,
                draggableLayer
            );

            return hit.collider
                ? hit.transform.GetComponent<AquariumFish>()
                : null;
        }

        private void StartGrab(AquariumFish fish, Vector2 mousePosition)
        {
            float z = fish.transform.position.z;
            if (!_cursorMapper.TryGetWorldPosition(mousePosition, z, out Vector2 mouseWorld))
                return;

            _targetFish = fish;
            _offset = (Vector2)fish.transform.position - mouseWorld;
            _dragging = true;

            fish.Grab();

            SFXManager.Instance.Play(SFXId.FishGrab);
        }

        private void Drag(Vector2 mousePosition)
        {
            if (!_dragging)
                return;

            float z = _targetFish.transform.position.z;
            if (!_cursorMapper.TryGetWorldPosition(mousePosition, z, out Vector2 mouseWorld))
                return;

            Vector2 position = mouseWorld + _offset;
            _targetFish.transform.position = new(position.x, position.y, z);
        }

        public void Drop()
        {
            if (!_dragging)
                return;

            OnDrop.Invoke(
                _targetFish.Fish,
                _targetFish.transform.position - transform.position
            );

            _targetFish.Release();
            _targetFish = null;
            _dragging = false;
        }
    }
}