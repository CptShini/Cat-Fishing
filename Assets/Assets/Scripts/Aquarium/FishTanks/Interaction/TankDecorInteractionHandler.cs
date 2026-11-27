using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Aquarium.FishTanks.Decor;
using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Fishies;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    [RequireComponent(typeof(AquariumCursorMapper))]
    public class TankDecorInteractionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask draggableLayer;
        
        private AquariumCursorMapper _cursorMapper;

        private PlacedTankDecorationObject _targetTankDecoration;
        private Vector2 _offset;

        public readonly UnityEvent<PlacedTankDecorationObject> OnSelectingTankDecor = new();

        private void Awake() => _cursorMapper = GetComponent<AquariumCursorMapper>();

        public void UpdateHover(Vector2 mousePosition)
        {

            if (!_cursorMapper.TryGetMouseRay(mousePosition, out var ray))
                return;


            PlacedTankDecorationObject hoveredTankDecor = GetTankDecorUnderMouse(ray);
            hoveredTankDecor?.Hovering();
        }
        
        public void Tick(Vector2 mousePosition, bool pressed)
        {
            if (pressed && _cursorMapper.TryGetMouseRay(mousePosition, out var ray))
            {
                var hoveredTankDecor = GetTankDecorUnderMouse(ray);
                if (hoveredTankDecor)
                    StartSelect(hoveredTankDecor);
            }
        }

        private PlacedTankDecorationObject GetTankDecorUnderMouse(Ray ray)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(
                ray,
                Mathf.Infinity,
                draggableLayer
            );

            return hit.collider
                ? hit.transform.GetComponent<PlacedTankDecorationObject>()
                : null;
        }

        private void StartSelect(PlacedTankDecorationObject placedTankDecoration)
        {
            _targetTankDecoration = placedTankDecoration;
            SFXManager.Instance.Play(SFXId.PickUpItem);
            OnSelectingTankDecor.Invoke(_targetTankDecoration);
        }
    }


}