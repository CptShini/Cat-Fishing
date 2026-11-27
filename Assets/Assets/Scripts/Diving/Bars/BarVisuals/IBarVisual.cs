using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public interface IBarVisual
    {
        void SetImage(Image image);
        void UpdateVisual(float fill);
    }
}