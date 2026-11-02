using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [System.Serializable]
    public struct Range
    {
        [Min(0)] public float Min;
        [Min(0)] public float Max;
        
        public float Average => (Min + Max) / 2f;
        public float Random() => UnityEngine.Random.Range(Min, Max);
    }
}