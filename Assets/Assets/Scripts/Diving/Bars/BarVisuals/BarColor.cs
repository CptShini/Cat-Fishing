using UnityEngine;

namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public class BarColor : BarVisual
    {
        [SerializeField] private Gradient gradient;
        
        public override void UpdateVisual(float p) =>
            Image.color = gradient.Evaluate(p);
    }
}