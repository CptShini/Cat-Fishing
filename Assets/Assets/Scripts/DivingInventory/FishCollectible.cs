using UnityEngine;

public class FishCollectible : Collectible
{
    public Fish fish;

    public FishCollectible(Fish fish)
    {
        this.fish = fish;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}
