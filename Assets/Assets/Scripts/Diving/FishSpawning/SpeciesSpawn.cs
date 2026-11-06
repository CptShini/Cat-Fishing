using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Diving.FishSpawning
{
    [Serializable]
    public class SpeciesSpawn
    {
        public FishSpecies Species;
        public int Weight;
    }
}