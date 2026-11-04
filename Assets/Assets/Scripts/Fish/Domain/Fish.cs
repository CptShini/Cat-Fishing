using System;

namespace Assets.Scripts.Fish.Domain
{
    public class Fish
    {
        public readonly Guid Id;
        public readonly FishSpecies Species;

        public readonly float Weight;
        public readonly float RelativeWeight;
        public readonly float Strength;

        public Fish(FishSpecies species)
        {
            Id = Guid.NewGuid();
            Species = species;

            Weight = species.Weight.Random();
            RelativeWeight = Weight / species.Weight.Average;
            Strength = species.Strength * RelativeWeight;
        }

        public Fish(FishDTO fishDTO, FishSpeciesDatabase db)
        {
            Id = Guid.Parse(fishDTO.Id);
            Species = db[fishDTO.SpeciesId];
            
            Weight = fishDTO.Weight;
            RelativeWeight = fishDTO.RelativeWeight;
            Strength = fishDTO.Strength;
        }
    }
}