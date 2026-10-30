using Assets.Scripts.PlayerState;
using UnityEngine;

public class CatchBar : StatBar
{

    public static CatchBar Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }
    
    void Update()
    {
       
    }

}
