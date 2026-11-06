using UnityEngine;

public class FurnitureVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
    }
}
