using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Aquarium.Domain
{
    [Serializable]
    public class TankFishDTO
    {
        public FishDTO Fish;
        public int Hunger;

        public TankFishDTO(TankFish tankFish)
        {
            Fish = new(tankFish.Fish);
            Hunger = tankFish.Hunger;
        }
    }
}