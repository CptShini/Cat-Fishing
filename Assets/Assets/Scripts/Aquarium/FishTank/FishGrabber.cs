using System;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishGrabber : MonoBehaviour
    {
        [SerializeField] private LayerMask draggableLayer;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private RawImage rawImage;

        private AquariumFish _targetFish;
        private Vector2 _offset;
        private Vector2 _mousePos;

        private bool _dragging;

        public readonly UnityEvent<Fish, Vector2> DropFish = new();

        private void Update()
        {
            var mouse = Mouse.current;
            _mousePos = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame) StartGrab();
            if (mouse.leftButton.isPressed) Drag();
            if (mouse.leftButton.wasReleasedThisFrame) Drop();
        }
        
        private void StartGrab()
        {
            if (!GetMouseRay(out var ray)) return;

            var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, draggableLayer);
            if (!hit.collider) return;

            var fish = hit.transform.GetComponent<AquariumFish>();
            if (!fish || !GetWorldPosition(ray, fish.transform.position.z, out var mouseWorld))
                return;

            _targetFish = fish;
            _offset = (Vector2)fish.transform.position - mouseWorld;
            _dragging = true;

            fish.Grab();
        }

        private void Drag()
        {
            if (!_dragging || !GetMouseRay(out var ray)) return;
            
            float z = _targetFish.transform.position.z;
            if (!GetWorldPosition(ray, z, out var mouseWorld)) return;

            var position = mouseWorld + _offset;
            _targetFish.transform.position = new(position.x, position.y, z);
        }

        private void Drop()
        {
            if (!_dragging) return;
            
            DropFish.Invoke(_targetFish.Fish, _targetFish.transform.position - transform.position);
            
            _targetFish.Release();
            _targetFish = null;
            _dragging = false;
        }

        private bool GetMouseRay(out Ray ray)
        {
            ray = default;
            var rectTransform = rawImage.rectTransform;

            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, _mousePos) ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, _mousePos, null, out var localPoint))
                return false;

            var rect = rectTransform.rect;
            var uvRect = rawImage.uvRect;

            var uv = new Vector2(
                uvRect.x + (localPoint.x - rect.xMin) / rect.width * uvRect.width,
                uvRect.y + (localPoint.y - rect.yMin) / rect.height * uvRect.height
            );

            ray = renderCamera.ViewportPointToRay(uv);
            return true;
        }

        private static bool GetWorldPosition(Ray ray, float z, out Vector2 worldPos)
        {
            var plane = new Plane(Vector3.forward, Vector3.forward * z);

            if (plane.Raycast(ray, out float distance))
            {
                worldPos = ray.GetPoint(distance);
                return true;
            }

            worldPos = default;
            return false;
        }

        private void OnEnable()
        {
            renderCamera.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            renderCamera.gameObject.SetActive(false);
            Drop();
        }
    }
}