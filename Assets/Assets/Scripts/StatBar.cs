
using System;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image foreground;
    public float maxValue { get; protected set; }
    public float currentValue { get; protected set; }

    public void SetCurrentValue(float value)
    {
        currentValue = value;
        foreground.fillAmount = currentValue / maxValue;
    }
    public void AddToCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(currentValue + value, 0, maxValue);
        foreground.fillAmount = currentValue / maxValue;
    }
    public void SetMaxValue(float value)
    {
        maxValue = value;
    }
}
