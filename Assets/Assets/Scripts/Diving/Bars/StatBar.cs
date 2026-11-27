using Assets.Scripts.Diving.Bars.BarVisuals;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Diving.Bars
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private BarVisual visual;
        
        private float _currentValue, _maxVal;
        
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnValueChanged();
            }
        }

        public float CurrentPercent { get; private set; }
        
        public float MaxVal
        {
            get => _maxVal;
            set
            {
                _maxVal = value;
                OnValueChanged();
            }
        }

        public readonly UnityEvent<float, float> ValueChanged = new();
        public readonly UnityEvent BarEmpty = new();
        
        private void OnValueChanged()
        {
            if (_currentValue <= 0f) BarEmpty.Invoke();
            
            _currentValue = Mathf.Clamp(_currentValue, 0f, MaxVal);
            CurrentPercent = _currentValue / MaxVal;
            
            visual?.UpdateVisual(CurrentPercent);

            ValueChanged.Invoke(_currentValue, MaxVal);
        }
    }
}