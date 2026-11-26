using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.Domain
{
    public class TankFish
    {
        public readonly Fish Fish;

        public int Age { get; private set; }
        public int Hunger { get; private set; }
        public int Happiness { get; private set; }
        public float Entertainment { get; private set; }
        
        private int _capacityPenalty;

        public readonly UnityEvent Fed = new();
        public readonly UnityEvent<TankFish, FishCauseOfDeath> Died = new();

        public TankFish(Fish fish)
        {
            Fish = fish;
            Hunger = 5;
        }
        
        public TankFish(TankFishDTO tankFishDTO, FishSpeciesDatabase db)
        {
            Fish = new(tankFishDTO.Fish, db);
            Age = tankFishDTO.Age;
            Hunger = tankFishDTO.Hunger;
            Recalculate();
        }

        public void DayStarted()
        {
            if (Hunger == 10) return;
            
            Hunger++;
            Recalculate();
        }
        
        public void DayEnded()
        {
            Age++;
            if (Hunger >= 10)
                Died.Invoke(this, FishCauseOfDeath.Hunger);
            else if (Fish.Species.Lifetime > 0 && Age >= Fish.Species.Lifetime) 
                Died.Invoke(this, FishCauseOfDeath.OldAge);
        }

        public void Feed()
        {
            if (Hunger <= 0) return;
            
            Hunger--;
            Recalculate();
            Fed.Invoke();
        }

        public void SetTankCrowdingInfo(int count, int capacity)
        {
            _capacityPenalty = 0;
            
            if (count == 1)
                _capacityPenalty = 3;
            else if (count > capacity)
                _capacityPenalty = (count - capacity) * 2;
            
            Recalculate();
        }
        
        private void Recalculate()
        {
            Happiness = Mathf.Clamp(10 - Hunger - _capacityPenalty, 0, 10);
            Entertainment = Mathf.Sqrt(
                Fish.Species.Rarity *
                Happiness *
                2.5f
            );
        }

        public void TankTooDirty() =>
            Died.Invoke(this, FishCauseOfDeath.Dirtiness);
    }
}