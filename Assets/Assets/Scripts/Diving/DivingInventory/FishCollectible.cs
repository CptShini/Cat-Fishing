using Assets.Scripts.Fish;

[System.Serializable]
public class FishCollectible : Collectible
{
    public Fish fish;

    public FishCollectible(Fish fish)
    {
        this.fish = fish;
        this.collectibleName = fish.fishName;
        this.weight = fish.weight;
        this.sprite = fish.sprite;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}
