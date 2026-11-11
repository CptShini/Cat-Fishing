using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Data.Domain
{
    public class TankFish
    {
        public readonly Fish Fish;

        public int Happiness { get; private set; }
        
        // Happiness
        // Per day: -1
        // When fed: +1

        public float Entertainment { get; private set; }

        public void DayPassed()
        {
            Happiness--;
            HappinessChanged();
        }

        public void Fed()
        {
            Happiness++;
            HappinessChanged();
        }
        
        private void HappinessChanged()
        {
            Entertainment = Mathf.Sqrt(
                Fish.Species.Rarity *
                Happiness *
                2.5f
            );
        }
    }
}