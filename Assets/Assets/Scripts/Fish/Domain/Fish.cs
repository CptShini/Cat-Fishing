using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Domain
{
    public class Fish
    {
        public Guid Id;
        public FishSpecies Species;

        public float Weight;
        public float Strength;

        public Fish(FishSpecies species)
        {
            Id = Guid.NewGuid();
            Species = species;
            
            Weight = Random.Range(species.Weight.Min, species.Weight.Max);
            Strength = species.Strength * (Weight / species.Weight.Average);
        }

        public Fish(FishDTO fishDTO, FishSpeciesDatabase db)
        {
            Id = Guid.Parse(fishDTO.Id);
            Species = db[fishDTO.SpeciesId];
            
            Weight = fishDTO.Weight;
            Strength = fishDTO.Strength;
        }
    }
}