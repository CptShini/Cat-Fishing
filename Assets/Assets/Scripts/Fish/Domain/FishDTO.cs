using System;

namespace Assets.Scripts.Fish.Domain
{
    [Serializable]
    public class FishDTO
    {
        public string Id;
        public int SpeciesId;
        
        public float Weight;
        public float Strength;
        
        public FishDTO() { }

        public FishDTO(Fish fish)
        {
            Id = fish.Id.ToString();
            SpeciesId = fish.Species.Id;
            Weight = fish.Weight;
            Strength = fish.Strength;
        }
    }
}