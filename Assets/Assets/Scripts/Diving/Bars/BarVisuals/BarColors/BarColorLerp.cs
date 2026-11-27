using UnityEngine;

namespace Assets.Scripts.Diving.Bars.BarVisuals.BarColors
{
    public class BarColorLerp : BarColor
    {
        [SerializeField] private Color 
            minColor,
            maxColor;
        
        protected override void SetColor(float fill) => 
            Image.color = Color.Lerp(minColor, maxColor, fill);
    }
}