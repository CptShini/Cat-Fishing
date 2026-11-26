using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] public Image foreground;
        [SerializeField] protected bool filledImage = true;
        [SerializeField] protected bool vertical = true;
        private RectTransform rt;
        
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnValueChanged();
            }
        }

        public float CurrentPercent => _currentValue / MaxVal;

        public readonly UnityEvent<float, float> ValueChanged = new();
        public readonly UnityEvent BarEmpty = new();
        
        private float _currentValue;
        private float _minValue;
        protected float MaxVal { get; private set; }

        private float maxLength, curLength;

        private void Awake()
        {
            rt = foreground.gameObject.GetComponent<RectTransform>();
            maxLength = vertical ? rt.sizeDelta.y : rt.sizeDelta.x;
            curLength = maxLength;
        }

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

            if (filledImage)
                UpdateFill();
            else
                UpdateSliced();

            ValueChanged.Invoke(_currentValue, MaxVal);
        }
        
        protected virtual void UpdateFill() =>
            foreground.fillAmount = _currentValue / MaxVal;
        protected virtual void UpdateSliced()
        {
            curLength = maxLength * (CurrentValue / MaxVal);
            rt.sizeDelta = vertical ? new(rt.sizeDelta.x, curLength) : new(curLength, rt.sizeDelta.y);
        }
    }
}