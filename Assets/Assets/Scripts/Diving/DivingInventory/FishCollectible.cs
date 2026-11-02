using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;

[System.Serializable]
public class FishCollectible : Collectible
{
    public FishDTO fishDto;

    public FishCollectible(FishDTO fishDto)
    {
        this.fishDto = fishDto;
        this.collectibleName = fishDto.Name;
        this.weight = fishDto.Weight;
        this.sprite = fishDto.sprite;
    }

    public override void Drop()
    {
        // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
    }

}
