using UnityEngine;

namespace Assets.Scripts.Fishies.Visuals
{
    public class FishVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
    
        public void Init(Sprite sprite, float size)
        {
            spriteRenderer.sprite = sprite;
            transform.localScale *= size;
        }
    }
}
