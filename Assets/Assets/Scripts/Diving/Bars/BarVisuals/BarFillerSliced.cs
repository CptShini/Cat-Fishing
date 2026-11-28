using UnityEngine;

namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public class BarFillerSliced : BarVisual
    {
        [SerializeField] private bool vertical = true;

        private RectTransform _foreground;

        private Vector2 _anchorMin, _anchorMax;
        private Vector2 _offsetMin, _offsetMax;
        
        private void Awake()
        {
            _foreground = Image.rectTransform;
            
            _anchorMin = _foreground.anchorMin;
            _anchorMax = _foreground.anchorMax;
            _offsetMin = _foreground.offsetMin;
            _offsetMax = _foreground.offsetMax;
        }

        public override void UpdateVisual(float p)
        {
            Vector2 anchorMax = _anchorMax;
            Vector2 offsetMax = _offsetMax;

            if (vertical)
            {
                anchorMax.y = Mathf.Lerp(_anchorMin.y, _anchorMax.y, p);
                offsetMax.y = Mathf.Lerp(_offsetMin.y, _offsetMax.y, p);
            }
            else
            {
                anchorMax.x = Mathf.Lerp(_anchorMin.x, _anchorMax.x, p);
                offsetMax.x = Mathf.Lerp(_offsetMin.x, _offsetMax.x, p);
            }

            _foreground.anchorMin = _anchorMin;
            _foreground.anchorMax = anchorMax;
            _foreground.offsetMin = _offsetMin;
            _foreground.offsetMax = offsetMax;
        }
    }
}