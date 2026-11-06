using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private Image foreground;
        
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnValueChanged();
            }
        }

        public readonly UnityEvent<float> ValueChanged = new();
        public readonly UnityEvent BarEmpty = new();
        
        private float _currentValue;
        private float _minValue;
        private protected float _maxValue;

        public void SetMaxValue(float maxVal, float newVal = -1f)
        {
            _maxValue = maxVal;
            _currentValue = newVal < 0 ? maxVal : newVal;
            UpdateFill();
        }
        
        public void SetMinValue(float minVal) => _minValue = minVal;

        private void OnValueChanged()
        {
            if (_currentValue <= _minValue) BarEmpty.Invoke();
            
            _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);
            UpdateFill();
            
            ValueChanged.Invoke(_currentValue);
        }
        
        private void UpdateFill() =>
            foreground.fillAmount = _currentValue / _maxValue;
    }
}