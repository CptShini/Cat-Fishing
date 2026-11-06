namespace Assets.Scripts.Helpers
{
    [System.Serializable]
    public struct Range
    {
        public float Min;
        public float Max;
        
        public float Average => (Min + Max) / 2f;
        public float Random() => UnityEngine.Random.Range(Min, Max);
    }
}