using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Diving.Bars
{
    public class ColorChangeBar : StatBar
    {
        [SerializeField] private Color minColor;
        [SerializeField] private Color maxColor;

        protected override void UpdateFill()
        {
            base.UpdateFill();
            foreground.color = Color.Lerp(minColor, maxColor, foreground.fillAmount);
        }
        protected override void UpdateSliced()
        {
            base.UpdateSliced();
            float ratio = CurrentValue / MaxVal;
            foreground.color = Color.Lerp(minColor, maxColor, ratio);
        }
    }
}