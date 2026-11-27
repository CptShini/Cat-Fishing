using Assets.Scripts.Diving.Bars.BarVisuals.BarColors;
using Assets.Scripts.Diving.Bars.BarVisuals.BarFillers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public class BarVisual : MonoBehaviour
    {
        [SerializeField] private Image barImage;
        [SerializeField] private BarFiller barFiller;
        [SerializeField] private BarColor barColor;

        private bool _imageSet;
        
        private void Awake()
        {
            barFiller?.SetImage(barImage);
            barColor?.SetImage(barImage);
            _imageSet = true;
        }

        public void UpdateVisual(float fill)
        {
            if (!_imageSet) return;
            
            barFiller?.UpdateVisual(fill);
            barColor?.UpdateVisual(fill);
        }
    }
}