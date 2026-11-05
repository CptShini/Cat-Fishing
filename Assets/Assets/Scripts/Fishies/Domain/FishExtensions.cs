using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;

namespace Assets.Scripts.Fishies.Domain
{
    public static class FishExtensions
    {
        public static IEnumerable<FishDTO> ToDTOs(this IEnumerable<Fish> fish) =>
            fish.Select(f => new FishDTO(f));

        public static IEnumerable<Fish> ToDomain(this IEnumerable<FishDTO> fishDTOs, FishSpeciesDatabase db) =>
            fishDTOs.Select(dto => new Fish(dto, db));

        public static Dictionary<Guid, Fish> ToDictionary(
            this IEnumerable<FishDTO> fishDTOs,
            FishSpeciesDatabase db
        ) {
            var dictionary = new Dictionary<Guid, Fish>();
            
            foreach (FishDTO fish in fishDTOs)
            {
                Guid key = Guid.Parse(fish.Id);
                Fish value = new(fish, db);
                dictionary.Add(key, value);
            }
            
            return dictionary;
        }
        
        public static Dictionary<Guid, Fish> ToDictionary(
            this SerializableDictionary<string, FishDTO> fishDic, 
            FishSpeciesDatabase db
        ) {
            var dictionary = new Dictionary<Guid, Fish>();
            
            foreach (string fishId in fishDic.Keys)
            {
                Guid key = Guid.Parse(fishId);
                Fish value = new(fishDic[fishId], db);
                dictionary.Add(key, value);
            }
            
            return dictionary;
        }

        public static SerializableDictionary<string, FishDTO> ToSerializableDictionary(
            this Dictionary<Guid, Fish> fishDic
        ) {
            var serializableDictionary = new SerializableDictionary<string, FishDTO>();
            
            foreach (Guid fishId in fishDic.Keys)
            {
                string key = fishId.ToString();
                FishDTO value = new(fishDic[fishId]);
                serializableDictionary.Add(key, value);
            }
            
            return serializableDictionary;
        }
    }
}