using UnityEngine;

namespace Assets.Scripts.Fish
{
    public class FishSpriteSetter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
    
        public void SetSprite(Sprite sprite) => 
            spriteRenderer.sprite = sprite;
    }
}
