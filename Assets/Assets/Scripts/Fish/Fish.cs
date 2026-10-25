public class Fish
{
    public FishSO fish;

    public FishCollectible(FishSO fish)
    {
        this.fish = fish;
        this.collectibleName = fish.fishName;
        this.sprite = fish.fishIcon;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}