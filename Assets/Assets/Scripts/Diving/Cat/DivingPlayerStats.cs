using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class DivingPlayerStats : MonoBehaviour, IDataPersistence
{
    public static DivingPlayerStats Instance { get; private set; }

    [Header("Speed")]
    public float speed = 10;
    public float stealthSpeed = 3;
    public float pounceSpeed = 20;

    [Header("Acceleration")]
    public float acceleration = 5; 
    public float friction = 5;
    
    [Header("Stamina")]
    public float maxStamina = 60;
    public float maxPounceStamina = 20;
    public float staminaRegen = 5;
    
    [Header("Strength")]
    public float strength = 10;  // How strong the cat is when grabbing fish

    [Header("Pouncing stats")]
    public float chargeUpSpeed = 10;  // per second
    public float pounceCooldown = 1; // seconds

    [Header("Oxygen")]
    public float oxygenCapacity = 100; // seconds

    [Header("Presence in Radius")]
    public float defaultPresence = 2;
    public float stealthPresence = 1;
    public float pouncePresence = 4;

    [Header("Other")]
    public float stunDuration = 2; // The amount of time in seconds player will be stunned
    public float carryingCapacity = 25f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void LoadData(GameData data)
    {
        if (data.PlayerStats == null)
            return;

        if (data.PlayerStats.TryGetValue("Speed", out float savedSpeed))
            speed = savedSpeed;

        if (data.PlayerStats.TryGetValue("PounceSpeed", out float savedPounceSpeed))
            pounceSpeed = savedPounceSpeed;

        if (data.PlayerStats.TryGetValue("ClawsStrength", out float savedClawsStrength))
            strength = savedClawsStrength;

        if (data.PlayerStats.TryGetValue("OxygenCapacity", out float savedOxygen))
            oxygenCapacity = savedOxygen;

        if (data.PlayerStats.TryGetValue("CarryingCapacity", out float savedCapacity))
            carryingCapacity = savedCapacity;
    }

    public void SaveData(ref GameData data)
    {
        if (data.PlayerStats == null)
            data.PlayerStats = new();

        data.PlayerStats["Speed"] = speed;
        data.PlayerStats["PounceSpeed"] = pounceSpeed;
        data.PlayerStats["ClawsStrength"] = strength;
        data.PlayerStats["OxygenCapacity"] = oxygenCapacity;
        data.PlayerStats["CarryingCapacity"] = carryingCapacity;
    }
}