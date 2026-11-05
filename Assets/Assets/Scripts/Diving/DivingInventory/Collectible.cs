using UnityEngine;

namespace Assets.Scripts.Diving.DivingInventory
{
    public class Collectible
    {
        public readonly string Name;
        public readonly Sprite Sprite;
        public readonly float Weight;
        
        public Collectible(string name, Sprite sprite, float weight)
        {
            Name = name;
            Sprite = sprite;
            Weight = weight;
        }
    
        public virtual void Drop() { }
    }
}
