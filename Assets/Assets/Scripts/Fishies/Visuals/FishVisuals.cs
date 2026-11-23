using UnityEngine;

namespace Assets.Scripts.Fishies.Visuals
{
    public class FishVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
    
        public void Init(Sprite sprite, float baseScale, float relativeWeight)
        {
            spriteRenderer.sprite = sprite;
            transform.localScale *= baseScale * Mathf.Sqrt(relativeWeight); // Cube-root is realistic, but whatevs
        }
    }
}
