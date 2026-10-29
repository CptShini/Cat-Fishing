using Assets.Scripts.PlayerState;
using UnityEngine;

public class StaminaBar : StatBar
{
    public static StaminaBar Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        maxValue = DivingPlayerStats.Instance.maxStamina;
        currentValue = maxValue;
    }

    void Update()
    {
        if(DivingController.Instance.state == PlayerState.SWIM)
        {
            StaminaBar.Instance.StaminaRegen(DivingPlayerStats.Instance.staminaRegen * Time.deltaTime);
        }
    }

    public void StaminaRegen(float amount)
    {
        
        SetCurrentValue(Mathf.Clamp(currentValue + amount, 0, DivingPlayerStats.Instance.maxStamina));
    }


}
