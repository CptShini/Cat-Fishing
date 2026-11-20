using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] protected Image foreground;
        [SerializeField] protected bool filledImage = true;
        [SerializeField] protected bool vertical = true;
        protected RectTransform rt;
        
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
        
        protected float _currentValue;
        protected float _minValue;
        protected float MaxVal { get; private set; }

        protected float maxLength;
        protected float curLength;

        void Awake()
        {
            rt = foreground.gameObject.GetComponent<RectTransform>();
            maxLength = vertical ? rt.sizeDelta.y : rt.sizeDelta.x;
            Debug.Log("AAHHH");
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
            {
                UpdateFill();
            }
            else
            {
                UpdateSliced();
            }           
            
            ValueChanged.Invoke(_currentValue, MaxVal);
        }
        
        protected virtual void UpdateFill() =>
            foreground.fillAmount = _currentValue / MaxVal;
        protected virtual void UpdateSliced()
        {
            curLength = maxLength * (CurrentValue / MaxVal);
            rt.sizeDelta = vertical ? new Vector2(rt.sizeDelta.x, curLength) : new Vector2(curLength, rt.sizeDelta.y);
        }
    }
}