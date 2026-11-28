using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public bool GetRandomSpawn(out T spawn)
        {
            spawn = default;
            if (Spawns.Length == 0) return false;
            
            if (EmptyWeight > 0)
            {
                int totalWeight = EmptyWeight + Spawns.Sum(ws => ws.Weight);
                int rand = Random.Range(0, totalWeight);
                if (rand < EmptyWeight) return false;
            }

            var spawnDict = Spawns.ToDictionary(ws => ws.Item, ws => ws.Weight);
            spawn = RandomHelper.WeightedRandom(spawnDict);
            return true;
        }
    }
}