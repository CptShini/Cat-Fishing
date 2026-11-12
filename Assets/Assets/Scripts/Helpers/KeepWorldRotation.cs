using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class KeepWorldRotation : MonoBehaviour
    {
        private Vector3 _worldOffset;
        private Transform _parent;
        private Quaternion _desiredWorldRotation;

        private void Start()
        {
            _worldOffset = transform.localPosition;
            _parent = transform.parent;
            _desiredWorldRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            if (!_parent) return;
            
            transform.localPosition = _parent.InverseTransformPoint(_parent.position + _worldOffset);
            transform.localRotation = Quaternion.Inverse(_parent.rotation) * _desiredWorldRotation;
        }
    }
}