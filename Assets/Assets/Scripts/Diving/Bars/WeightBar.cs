using UnityEngine;

namespace Assets.Scripts.Diving.Bars
{
    public class WeightBar : StatBar
    {
        private Animator _animator;

        private void Awake() => _animator = GetComponent<Animator>();

        public bool TryAdd(float weight)
        {
            bool tooHeavy = CurrentValue + weight > _maxValue;
            if (tooHeavy) return false;
            
            CurrentValue += weight;
            return true;
        }
        public bool CanAdd(float weight)
        {
            bool tooHeavy = CurrentValue + weight > _maxValue;
            if (!tooHeavy) return true;
            
            _animator.Play("WeightWarn");
            return false;

        }
    }
}