using UnityEngine;

[System.Serializable]
public class Fish
{
    public string fishName;
    public Sprite sprite;
    public float weight;


    public Fish(FishSO fish)
    {
        this.fishName = fish.fishName;
        this.sprite = fish.fishIcon;
        this.weight = Random.Range(fish.weightMin, fish.weightMax);
    }
}