using UnityEngine;

namespace Assets.Scripts.Fish
{
    public class FishSpriteSetter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
    
        public void SetSprite(Sprite sprite) => 
            _spriteRenderer.sprite = sprite;
    }
}
