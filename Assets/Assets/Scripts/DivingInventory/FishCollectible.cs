using UnityEngine;

public class FishCollectible : Collectible
{
    public FishSO fish;

    public FishCollectible(FishSO fish)
    {
        this.fish = fish;
        this.collectibleName = fish.fishName;
        this.sprite = fish.sprite;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}
