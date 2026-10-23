using UnityEngine;
using UnityEngine.UI;

public class Collectible
{
    public string collectibleName;
    public Sprite sprite;
    public float weight;

    public virtual void Drop() { return; }
    
}
