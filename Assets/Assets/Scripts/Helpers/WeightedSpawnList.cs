using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public class WeightedSpawn<T>
    {
        public T Item;
        public int Weight;
    }
    
    [Serializable]
    public class WeightedSpawnList<T>
    {
        [Min(0)] public int EmptyWeight;
        
        public WeightedSpawn<T>[] Spawns;

        public T GetRandomSpawn()
        {
            if (EmptyWeight > 0)
            {
                
            }

            Dictionary<T, int> spawnDict = Spawns
                .ToDictionary(s => s.Item, s => s.Weight);
            return RandomHelper.WeightedRandom(spawnDict);
        }
    }
}