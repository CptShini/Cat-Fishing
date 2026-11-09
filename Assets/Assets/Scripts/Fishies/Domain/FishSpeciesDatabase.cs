using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Fishies.Domain
{
    [CreateAssetMenu(fileName = "New Species Database", menuName = "ScriptableObjects/Fish/Species Database")]
    public class FishSpeciesDatabase : ScriptableObject
    {
        [SerializeField] private List<FishSpecies> species;
        
        private Dictionary<int, FishSpecies> _lookup;

        public IReadOnlyCollection<FishSpecies> GetSpecies()
        {
            return species;
        }

        public FishSpecies this[int id] =>
            species
                .ToDictionary(s => s.Id)
                .TryGetValue(id, out FishSpecies result) 
                ? result 
                : throw new KeyNotFoundException($"Unknown FishSpecies ID: {id}");
    }
}