using System;

namespace Assets.Scripts.Fishies.Domain
{
    [Serializable]
    public class FishDTO
    {
        public string Id;
        public int SpeciesId;
        public float Weight;

        public FishDTO(Fish fish)
        {
            Id = fish.Id.ToString();
            SpeciesId = fish.Species.Id;
            Weight = fish.Weight;
        }
    }
}