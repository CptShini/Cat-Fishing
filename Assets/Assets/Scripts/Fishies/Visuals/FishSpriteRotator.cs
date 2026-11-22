using System;
using UnityEngine;

namespace Assets.Scripts.Fishies.Visuals
{
    [ExecuteInEditMode]
    public class FishSpriteRotator : MonoBehaviour
    {
        private bool _regularRotation;

        private Transform _transform, _parentTransform;

        private void Awake()
        {
            _transform = transform;
            _parentTransform = transform.parent;
        }

        private void Start() => 
            _regularRotation = _parentTransform.right.x < 0;

        private void Update()
        {
            Vector3 localScale = _transform.localScale;
            switch (_regularRotation)
            {
                case false when _parentTransform.right.x >= 0:
                    localScale.y = Mathf.Abs(localScale.y);
                    _regularRotation = true;
                    break;
                case true when _parentTransform.right.x < 0:
                    localScale.y = -Mathf.Abs(localScale.y);
                    _regularRotation = false;
                    break;
            }
            _transform.localScale = localScale;
        }
    }
}
