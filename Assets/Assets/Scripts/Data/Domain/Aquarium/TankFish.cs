using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Data.Domain.Aquarium
{
    public class TankFish
    {
        public readonly Fish Fish;

        private int _hunger;
        public int Hunger
        {
            get => _hunger;
            private set
            {
                _hunger = value;
                _hunger = Mathf.Clamp(_hunger, 0, 10);
                Recalculate();
            }
        }
        
        private int _capacityPenalty;
        
        public int Happiness { get; private set; }
        public float Entertainment { get; private set; }

        public TankFish(Fish fish)
        {
            Fish = fish;
            Hunger = 5;
        }
        
        public TankFish(TankFishDTO tankFishDTO, FishSpeciesDatabase db)
        {
            Fish = new(tankFishDTO.Fish, db);
            Happiness = tankFishDTO.Happiness;
        }

        public void DayPassed() => Hunger++;
        public void Feed() => Hunger--;
        
        public void SetTankCrowdingInfo(int count, int capacity)
        {
            _capacityPenalty = 0;
            
            if (count == 1)
                _capacityPenalty = 3;
            else if (count > capacity)
                _capacityPenalty = count - capacity;
            
            Recalculate();
        }
        
        private void Recalculate()
        {
            Happiness = Mathf.Clamp(10 - _hunger - _capacityPenalty, 0, 10);
            Entertainment = Mathf.Sqrt(
                Fish.Species.Rarity *
                Happiness *
                2.5f
            );
        }
    }
}