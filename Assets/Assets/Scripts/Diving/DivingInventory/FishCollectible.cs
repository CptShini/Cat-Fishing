using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Diving.DivingInventory
{
    public class FishCollectible : Collectible
    {
        public readonly Fish fish;
        
        public FishCollectible(Fish fish)
            : base(fish.Species.Name, fish.Species.Icon, fish.Weight)
        {
            this.fish = fish;
        }

        public override void Drop()
        {
            // SOMETHING HAPPENS WHEN WE DROP A FISH OUT OF INVENTORY
        }
    }
}
