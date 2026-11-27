using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarColors
{
    public abstract class BarColor : MonoBehaviour, IBarVisual
    {
        protected Image Image;
        
        public void SetImage(Image image) => Image = image;
        public void UpdateVisual(float fill) => SetColor(fill);

        protected abstract void SetColor(float fill);
    }
}