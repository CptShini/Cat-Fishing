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

        public readonly UnityEvent<float, float> ValueChanged = new();
        public readonly UnityEvent BarEmpty = new();
        
        private float _currentValue;
        private float _minValue;
        protected float MaxVal { get; private set; }

        public void SetMaxValue(float maxVal, float newVal = -1f)
        {
            MaxVal = maxVal;
            _currentValue = newVal < 0 ? maxVal : newVal;
            OnValueChanged();
        }
        
        public void SetMinValue(float minVal) => _minValue = minVal;

        private void OnValueChanged()
        {
            if (_currentValue <= _minValue) BarEmpty.Invoke();
            
            _currentValue = Mathf.Clamp(_currentValue, _minValue, MaxVal);
            UpdateFill();
            
            ValueChanged.Invoke(_currentValue, MaxVal);
        }
        
        private void UpdateFill() =>
            foreground.fillAmount = _currentValue / MaxVal;
    }
}