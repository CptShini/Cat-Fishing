namespace Assets.Scripts.Diving.Bars
{
    public class WeightBar : StatBar
    {
        public bool TryAdd(float weight)
        {
            bool tooHeavy = CurrentValue + weight > _maxValue;
            if (tooHeavy) return false;
            
            CurrentValue += weight;
            return true;
        }
    }
}