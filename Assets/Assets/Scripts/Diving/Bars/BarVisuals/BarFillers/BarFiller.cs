using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarFillers
{
    public abstract class BarFiller : MonoBehaviour, IBarVisual
    {
        protected Image Image { get; private set; }
        
        public virtual void SetImage(Image image) => Image = image;
        public void UpdateVisual(float fill) => SetFill(fill);

        protected abstract void SetFill(float fill);
    }
}