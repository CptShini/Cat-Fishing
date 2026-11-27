namespace Assets.Scripts.Diving.Bars.BarVisuals.BarFillers
{
    public class BarFillerFilled : BarFiller
    {
        protected override void SetFill(float fill) => 
            Image.fillAmount = fill;
    }
}