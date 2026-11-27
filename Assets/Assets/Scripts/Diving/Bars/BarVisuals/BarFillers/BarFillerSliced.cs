using UnityEngine;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarFillers
{
    public class BarFillerSliced : BarFiller
    {
        [SerializeField] private bool vertical = true;

        private float _width;
        private float _maxLength;
        
        private RectTransform _foreground;
        
        private void Start()
        {
            _foreground = Image.rectTransform;
            _maxLength = vertical
                ? _foreground.sizeDelta.y
                : _foreground.sizeDelta.x;
            _width = vertical
                ? _foreground.sizeDelta.x
                : _foreground.sizeDelta.y;
        }

        protected override void SetFill(float fill)
        {
            float newLength = _maxLength * fill;
            _foreground.sizeDelta = vertical
                ? new(_width, newLength)
                : new(newLength, _width);
        }
    }
}