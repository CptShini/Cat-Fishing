using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Fish.Domain
{
    [CreateAssetMenu]
    public class FishSpeciesDatabase : ScriptableObject
    {
        [SerializeField] private List<FishSpecies> species;
        
        private Dictionary<int, FishSpecies> _lookup = new();

        public FishSpecies this[int id]
        {
            get
            {
                _lookup ??= species.ToDictionary(s => s.Id);
                
                return _lookup.TryGetValue(id, out FishSpecies result) 
                    ? result 
                    : throw new KeyNotFoundException($"Unknown FishSpecies ID: {id}");
            }
        }
    }
}