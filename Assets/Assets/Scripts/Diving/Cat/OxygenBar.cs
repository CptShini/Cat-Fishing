using Assets.Scripts.PlayerState;
using UnityEngine;

public class OxygenBar : StatBar
{
    public static OxygenBar Instance { get; private set; }
    private bool noOxygen = false;

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }
    void Start()
    {
        maxValue = DivingPlayerStats.Instance.oxygenCapacity;
        currentValue = maxValue;
    }

    void ReduceOxygen(float amount)
    {
        currentValue -= amount; 
        foreground.fillAmount = currentValue / maxValue;

        if(currentValue >= 0 || noOxygen)
        {
            return;
        }

        noOxygen = true;
        SceneTransition.Instance.LoadScene("Aquarium");
    }

    void Update()
    {
        if (DivingController.Instance.state != PlayerState.GRAB) // You cannot lose oxygen while battling a fish
        {
            ReduceOxygen(Time.deltaTime);
        }
    }

}
