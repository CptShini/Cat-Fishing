using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Fishies.Spawning
{
    [Serializable]
    public class SpeciesSpawn
    {
        public FishSpecies Species;
        public int Weight;
    }
}