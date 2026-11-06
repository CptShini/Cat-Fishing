using Assets.Scripts.Fishies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishGrabber : MonoBehaviour
    {
        [SerializeField] private LayerMask draggableLayer;
        [SerializeField] private Camera cam;

        private bool _dragging;
        private Vector2 _mousePosScreen, _mousePosWorld;
        
        private AquariumFish _targetFish;
        private Vector2 _offset;
        
        private void Update()
        {
            _mousePosScreen = Mouse.current.position.ReadValue();
            _mousePosWorld = cam.ScreenToWorldPoint(_mousePosScreen);
            
            StartGrab();
            Drag();
            Drop();
        }
        
        private void StartGrab()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            
            Ray ray = cam.ScreenPointToRay(_mousePosScreen);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, draggableLayer);
            if (!hit.collider) return;
            
            _dragging = true;
            _targetFish = hit.transform.GetComponent<AquariumFish>();
            _offset = (Vector2)_targetFish.transform.position - _mousePosWorld;
            
            _targetFish.Grab();
        }

        private void Drag()
        {
            if (!Mouse.current.leftButton.isPressed || !_dragging) return;

            float 
                x = _mousePosWorld.x + _offset.x,
                y = _mousePosWorld.y + _offset.y,
                z = _targetFish.transform.position.z;
            _targetFish.transform.position = new(x, y, z);
        }

        private void Drop()
        {
            if (!Mouse.current.leftButton.wasReleasedThisFrame) return;
            
            _targetFish.Release();
            
            _dragging = false;
            _targetFish = null;
        }
    }
}