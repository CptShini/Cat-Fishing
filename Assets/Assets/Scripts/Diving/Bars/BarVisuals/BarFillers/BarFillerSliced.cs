using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarFillers
{
    public class BarFillerSliced : BarFiller
    {
        [SerializeField] private bool vertical = true;

        private RectTransform _foreground;

        private Vector2 _anchorMin, _anchorMax;
        private Vector2 _offsetMin, _offsetMax;
        
        public override void SetImage(Image image)
        {
            base.SetImage(image);
            _foreground = Image.rectTransform;
            
            _anchorMin = _foreground.anchorMin;
            _anchorMax = _foreground.anchorMax;
            _offsetMin = _foreground.offsetMin;
            _offsetMax = _foreground.offsetMax;
        }

        protected override void SetFill(float fill)
        {
            Vector2 anchorMax = _anchorMax;
            Vector2 offsetMax = _offsetMax;

            if (vertical)
            {
                anchorMax.y = Mathf.Lerp(_anchorMin.y, _anchorMax.y, fill);
                offsetMax.y = Mathf.Lerp(_offsetMin.y, _offsetMax.y, fill);
            }
            else
            {
                anchorMax.x = Mathf.Lerp(_anchorMin.x, _anchorMax.x, fill);
                offsetMax.x = Mathf.Lerp(_offsetMin.x, _offsetMax.x, fill);
            }

            _foreground.anchorMin = _anchorMin;
            _foreground.anchorMax = anchorMax;
            _foreground.offsetMin = _offsetMin;
            _foreground.offsetMax = offsetMax;
        }
    }
}