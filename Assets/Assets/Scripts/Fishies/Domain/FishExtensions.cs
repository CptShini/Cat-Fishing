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
        ) =>
            fishDTOs.ToDictionary(
                dto => Guid.Parse(dto.Id),
                dto => new Fish(dto, db)
            );

        public static Dictionary<Guid, Fish> ToDictionary(
            this SerializableDictionary<string, FishDTO> fishDic, 
            FishSpeciesDatabase db
        ) =>
            fishDic.ToDictionary(
                kvp => Guid.Parse(kvp.Key),
                kvp => new Fish(kvp.Value, db)
            );

        public static SerializableDictionary<string, FishDTO> ToSerializableDictionary(
            this Dictionary<Guid, Fish> fishDic
        )
        {
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