using Assets.Scripts.Fish.Domain;

[System.Serializable]
public class FishCollectible : Collectible
{
    public Fish fish { get; private set; }

    public FishCollectible(Fish fish)
    {
        this.fish = fish;
        collectibleName = fish.Species.Name;
        weight = fish.Weight;
        sprite = fish.Species.Icon;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}
