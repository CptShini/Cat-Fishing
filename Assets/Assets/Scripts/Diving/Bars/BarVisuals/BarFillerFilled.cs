namespace Assets.Scripts.Diving.Bars.BarVisuals
{
    public class BarFillerFilled : BarVisual
    {
        public override void UpdateVisual(float p) =>
            Image.fillAmount = p;
    }
}