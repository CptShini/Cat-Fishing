using UnityEngine;

[System.Serializable]
public class Fish
{
    public string fishName;
    public Sprite sprite;
    public float weight;
    public float strength;

    public Fish(FishSO fish)
    {
        this.fishName = fish.fishName;
        this.sprite = fish.fishIcon;
        this.weight = Random.Range(fish.weightMin, fish.weightMax);

        float avg = (fish.weightMin + fish.weightMax) / 2;
        this.strength = fish.defaultStrength * (weight / avg);
    }
}