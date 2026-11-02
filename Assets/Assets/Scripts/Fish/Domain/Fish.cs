using System;

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

            Weight = species.Weight.Random();
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