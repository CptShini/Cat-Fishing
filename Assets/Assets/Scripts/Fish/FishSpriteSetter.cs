using UnityEngine;

public class FishSpriteSetter : MonoBehaviour
{
    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
