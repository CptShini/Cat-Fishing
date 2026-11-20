using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTanks.Interaction
{
    public class AquariumCursorMapper : MonoBehaviour
    {
        [SerializeField] private Camera renderCamera;
        [SerializeField] private RawImage rawImage;

        [field: SerializeField] public float InteractionPlaneZ { get; private set; }

        public bool TryGetMouseRay(Vector2 screenPosition, out Ray ray)
        {
            ray = default;
            
            RectTransform rectTransform = rawImage.rectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    rectTransform, screenPosition) ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, screenPosition, null, out Vector2 localPoint))
                return false;

            Rect rect = rectTransform.rect;
            Rect uvRect = rawImage.uvRect;
            Vector2 uv = new(
                uvRect.x + (localPoint.x - rect.xMin) / rect.width * uvRect.width,
                uvRect.y + (localPoint.y - rect.yMin) / rect.height * uvRect.height
            );

            ray = renderCamera.ViewportPointToRay(uv);
            return true;
        }

        public bool TryGetWorldPosition(Vector2 screenPosition, out Vector2 worldPosition) =>
            TryGetWorldPosition(screenPosition, InteractionPlaneZ, out worldPosition);
        
        public bool TryGetWorldPosition(Vector2 screenPosition, float z, out Vector2 worldPosition)
        {
            worldPosition = default;

            return 
                TryGetMouseRay(screenPosition, out Ray ray) && 
                TryGetWorldPosition(ray, z, out worldPosition);
        }

        private static bool TryGetWorldPosition(Ray ray, float z, out Vector2 worldPosition)
        {
            Plane plane = new(Vector3.forward, Vector3.forward * z);
            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
                return true;
            }

            worldPosition = default;
            return false;
        }

        public void SetRenderCameraActive(bool active) => 
            renderCamera.gameObject.SetActive(active);
    }
}