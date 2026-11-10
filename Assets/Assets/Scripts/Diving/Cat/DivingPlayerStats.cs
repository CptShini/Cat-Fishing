using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Upgrades;

public class DivingPlayerStats : MonoBehaviour, IDataPersistence
{
    public static DivingPlayerStats Instance { get; private set; }

    [SerializeField] private UpgradeLedger upgradeLedger;

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
        speed = GetStat(LevelUpgrade.Speed);
        pounceSpeed = GetStat(LevelUpgrade.PounceSpeed);
        strength = GetStat(LevelUpgrade.ClawStrength);
        oxygenCapacity = GetStat(LevelUpgrade.OxygenCapacity);
        carryingCapacity = GetStat(LevelUpgrade.CarryingCapacity);

        return;

        float GetStat(LevelUpgrade upgrade)
        {
            int statLevel = data.LevelUpgrades[upgrade];
            UpgradeLevel[] upgradeLevels = upgradeLedger.GetUpgradeLevels(upgrade);
            return upgradeLevels[statLevel].Stat;
        }
    }

    public void SaveData(ref GameData data)
    {
        // Nothing to save
    }
}