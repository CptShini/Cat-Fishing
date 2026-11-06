using UnityEngine;

namespace Assets.Scripts.Fishies.Visuals
{
    public class FishVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
    
        public void Init(Sprite sprite, float relativeWeight)
        {
            spriteRenderer.sprite = sprite;
            transform.localScale *= Mathf.Sqrt(relativeWeight); // Cube-root is realistic, but whatevs
        }
    }
}
