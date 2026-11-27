using UnityEngine;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarColors
{
    public class BarColorThreshold : BarColor
    {
        [SerializeField, Range(0f, 1f)] private float threshold;
        [SerializeField] private Color
            defaultColor,
            thresholdColor;

        protected override void SetColor(float fill)
        {
            Image.color = fill >= threshold
                ? defaultColor
                : thresholdColor;
        }
    }
}