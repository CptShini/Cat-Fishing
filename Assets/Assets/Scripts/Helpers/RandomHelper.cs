using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Helpers
{
    public static class RandomHelper
    {
        public static T WeightedRandom<T>(Dictionary<T, int> weights)
        {
            int weightSum = weights.Values.Sum();
            int rand = Random.Range(0, weightSum);

            int low = 0;
            foreach (T val in weights.Keys)
            {
                int high = low + weights[val];
                if (rand >= low && rand < high)
                    return val;
                
                low = high;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}