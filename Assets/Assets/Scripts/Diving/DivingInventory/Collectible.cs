using UnityEngine;

[System.SerializableAttribute]
public class Collectible
{
    public string collectibleName;
    public Sprite sprite;
    public float weight;


    public virtual void Drop() { return; }
    
}
