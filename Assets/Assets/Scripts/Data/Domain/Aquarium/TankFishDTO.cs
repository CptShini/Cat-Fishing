using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain.Aquarium
{
    [Serializable]
    public class TankFishDTO
    {
        public FishDTO Fish;
        public int Happiness;

        public TankFishDTO(TankFish tankFish)
        {
            Fish = new(tankFish.Fish);
            Happiness = tankFish.Happiness;
        }
    }
}