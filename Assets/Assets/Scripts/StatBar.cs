
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image foreground;
    private float maxValue;
    [SerializeField, Range(0f, 100f)] private float currentValue;

    public void SetCurrentValue(float value)
    {
        currentValue = value;
        foreground.fillAmount = currentValue / maxValue;
    }
    public void SetMaxValue(float value)
    {
        maxValue = value;
    }

    void Update()
    {
        // placeholder logic
        //foreground.fillAmount = currentValue / maxValue;
    }
}
