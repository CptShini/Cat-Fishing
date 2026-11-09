using UnityEngine;

namespace Assets.Scripts.Fishies.Visuals
{
    [ExecuteInEditMode]
    public class FishSpriteRotator : MonoBehaviour
    {
        private bool _regularRotation;

        private void Start() => 
            _regularRotation = transform.parent.right.x < 0;

        private void Update()
        {
            Vector3 localScale = transform.localScale;
            switch (_regularRotation)
            {
                case false when transform.parent.right.x >= 0:
                    localScale.y = Mathf.Abs(localScale.y);
                    _regularRotation = true;
                    break;
                case true when transform.parent.right.x < 0:
                    localScale.y = -Mathf.Abs(localScale.y);
                    _regularRotation = false;
                    break;
            }
            transform.localScale = localScale;
        }
    }
}
