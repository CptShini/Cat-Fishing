using System;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public string fishId;
    public string fishSOId;
    public string fishName;
    public Sprite sprite;
    public float weight;
    
    // Required for restoring saved fish.
    public Fish()
    {
    }

    public Fish(FishSO fish)
    {
        fishId = Guid.NewGuid().ToString();
        fishSOId = fish.fishKey;
        this.fishName = fish.fishName;
        this.sprite = fish.fishIcon;
        this.weight = UnityEngine.Random.Range(fish.weightMin, fish.weightMax);
    }
}