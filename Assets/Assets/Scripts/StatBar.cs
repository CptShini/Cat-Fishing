
using System;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image foreground;
    public float maxValue { get; private set; }
    public float currentValue { get; private set; }

    public void SetCurrentValue(float value)
    {
        currentValue = value;
        foreground.fillAmount = currentValue / maxValue;
    }
    public void AddToCurrentValue(float value)
    {
        Debug.Log(value + " + " + currentValue);
        currentValue = Mathf.Clamp(currentValue + value, 0, maxValue);
        Debug.Log(currentValue);

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
