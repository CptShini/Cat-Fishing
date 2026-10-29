using Assets.Scripts.PlayerState;
using UnityEngine;

public class OxygenBar : StatBar
{

    public static OxygenBar Instance { get; private set; }

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
        AddToCurrentValue(-amount);        
        if(currentValue >= 0)
        {
            return;
        }
        
        SetCurrentValue(float.MaxValue);
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
