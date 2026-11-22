using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Helpers
{
    public static class MousePositionHelper
    {
        public static bool TryGetMouseWorldPosition(this Transform t, Camera camera, out Vector3 mouseWorldPosition)
        {
            mouseWorldPosition = Vector3.zero;
            if (Mouse.current == null || !camera)
                return false;

            Ray mouseRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane aimPlane = new(Vector3.forward, t.position);
            if (!aimPlane.Raycast(mouseRay, out float distance))
                return false;

            mouseWorldPosition = mouseRay.GetPoint(distance);
            return true;
        }
    }
}