using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public enum PlayerUpgradeType
{
    Speed,
    PounceSpeed,
    ClawsStrength,
    OxygenCapacity,
    CarryingCapacity
}

public class ShopStatsData : MonoBehaviour, IDataPersistence
{
    [Header("Player Stats")]
    public float speed = 10f;
    public float pounceSpeed = 20f;
    public float clawsStrength = 10f;
    public float oxygenCapacity = 100f;
    public float carryingCapacity = 25f;

    public bool hasMiniMap { get; private set; }

    [Header("Upgrade Amounts")]
    [SerializeField] private float speedUpgrade = 2f;
    [SerializeField] private float pounceSpeedUpgrade = 2f;
    [SerializeField] private float clawsStrengthUpgrade = 2f;
    [SerializeField] private float oxygenUpgrade = 20f;
    [SerializeField] private float carryingCapacityUpgrade = 5f;

    [Header("Upgrade Levels")]
    public int speedLevel { get; private set; }
    public int pounceSpeedLevel { get; private set; }
    public int clawsStrengthLevel { get; private set; }
    public int oxygenCapacityLevel { get; private set; }
    public int carryingCapacityLevel { get; private set; }

    private int MaxUpgradeLevel = 5;

    [Header("Upgrade Base Prices")]
    [SerializeField] private int[] upgradePrices = { 10, 30, 65, 250, 400 };
    [SerializeField] private int miniMapPrice = 100;

    private float GetStat(GameData data, string key, float defaultValue)
    {
        if (!data.PlayerStats.ContainsKey(key))
        {
            data.PlayerStats[key] = defaultValue;
        }

        return data.PlayerStats[key];
    }

    public float GetStatValue(PlayerUpgradeType type)
    {
        switch (type)
        {
            case PlayerUpgradeType.Speed:
                return speed;

            case PlayerUpgradeType.PounceSpeed:
                return pounceSpeed;

            case PlayerUpgradeType.ClawsStrength:
                return clawsStrength;

            case PlayerUpgradeType.OxygenCapacity:
                return oxygenCapacity;

            case PlayerUpgradeType.CarryingCapacity:
                return carryingCapacity;

            default:
                return 0f;
        }
    }

    public int GetUpgradeLevel(PlayerUpgradeType type)
    {
        switch (type)
        {
            case PlayerUpgradeType.Speed:
                return speedLevel;

            case PlayerUpgradeType.PounceSpeed:
                return pounceSpeedLevel;

            case PlayerUpgradeType.ClawsStrength:
                return clawsStrengthLevel;

            case PlayerUpgradeType.OxygenCapacity:
                return oxygenCapacityLevel;

            case PlayerUpgradeType.CarryingCapacity:
                return carryingCapacityLevel;

            default:
                return 0;
        }
    }

    public void UpgradeStat(PlayerUpgradeType type)
    {
        if (GetUpgradeLevel(type) >= MaxUpgradeLevel)
        {
            Debug.Log($"{type} is already at max level");
            return;
        }

        int cost = GetUpgradeCost(type);

        if (!GoldManager.Instance.SpendGold(cost))
        {
            Debug.Log("Not enough gold!");
            return;
        }

        switch (type)
        {
            case PlayerUpgradeType.Speed:
                speed += speedUpgrade;
                speedLevel++;
                break;

            case PlayerUpgradeType.PounceSpeed:
                pounceSpeed += pounceSpeedUpgrade;
                pounceSpeedLevel++;
                break;

            case PlayerUpgradeType.ClawsStrength:
                clawsStrength += clawsStrengthUpgrade;
                clawsStrengthLevel++;
                break;

            case PlayerUpgradeType.OxygenCapacity:
                oxygenCapacity += oxygenUpgrade;
                oxygenCapacityLevel++;
                break;

            case PlayerUpgradeType.CarryingCapacity:
                carryingCapacity += carryingCapacityUpgrade;
                carryingCapacityLevel++;
                break;

            default:
                return;
        }

        SaveUpgrade();
    }

    public void PurchaseMiniMap()
    {
        if (hasMiniMap)
        {
            Debug.Log("Minimap is already purchased");
            return;
        }

        if (!GoldManager.Instance.SpendGold(miniMapPrice))
        {
            Debug.Log("Not enough gold to purchase MiniMap!");
            return;
        }

        hasMiniMap = true;
        SaveUpgrade();

        Debug.Log("Minimap purchased");
    }

    public int GetMiniMapPrice()
    {
        return miniMapPrice;
    }

    public int GetUpgradeCost(PlayerUpgradeType type)
    {
        int level = GetUpgradeLevel(type);

        if (level >= MaxUpgradeLevel)
            return 0;

        return upgradePrices[level];
    }

    private void SaveUpgrade()
    {
        SaveManager.Instance.SaveData();
        SaveManager.Instance.Save();
    }

        public void LoadData(GameData data)
    {
        if (data.PlayerStats == null)
            data.PlayerStats = new();

        speed = GetStat(data, "Speed", 10f);
        pounceSpeed = GetStat(data, "PounceSpeed", 20f);
        clawsStrength = GetStat(data, "ClawsStrength", 10f);
        oxygenCapacity = GetStat(data, "OxygenCapacity", 100f);
        carryingCapacity = GetStat(data, "CarryingCapacity", 25f);

        speedLevel = Mathf.Clamp(Mathf.RoundToInt(GetStat(data, "SpeedLevel", 0f)), 0, MaxUpgradeLevel);
        pounceSpeedLevel = Mathf.Clamp(Mathf.RoundToInt(GetStat(data, "PounceSpeedLevel", 0f)), 0, MaxUpgradeLevel);
        clawsStrengthLevel = Mathf.Clamp(Mathf.RoundToInt(GetStat(data, "ClawsStrengthLevel", 0f)), 0, MaxUpgradeLevel);
        oxygenCapacityLevel = Mathf.Clamp(Mathf.RoundToInt(GetStat(data, "OxygenCapacityLevel", 0f)), 0, MaxUpgradeLevel);
        carryingCapacityLevel = Mathf.Clamp(Mathf.RoundToInt(GetStat(data, "CarryingCapacityLevel", 0f)), 0, MaxUpgradeLevel);

        hasMiniMap = GetStat(data, "HasMiniMap", 0f) >= 1f;
    }

    public void SaveData(ref GameData data)
    {
        if (data.PlayerStats == null)
            data.PlayerStats = new();

        data.PlayerStats["Speed"] = speed;
        data.PlayerStats["PounceSpeed"] = pounceSpeed;
        data.PlayerStats["ClawsStrength"] = clawsStrength;
        data.PlayerStats["OxygenCapacity"] = oxygenCapacity;
        data.PlayerStats["CarryingCapacity"] = carryingCapacity;

        data.PlayerStats["SpeedLevel"] = speedLevel;
        data.PlayerStats["PounceSpeedLevel"] = pounceSpeedLevel;
        data.PlayerStats["ClawsStrengthLevel"] = clawsStrengthLevel;
        data.PlayerStats["OxygenCapacityLevel"] = oxygenCapacityLevel;
        data.PlayerStats["CarryingCapacityLevel"] = carryingCapacityLevel;

        data.PlayerStats["HasMiniMap"] = hasMiniMap ? 1f : 0f;
    }


    // just to test 😀👍🔥✨
    // (˵◝ ⩊  ◜˵マ meow uwu
    public void ResetStats()
    {
        speed = 10f;
        pounceSpeed = 20f;
        clawsStrength = 10f;
        oxygenCapacity = 100f;
        carryingCapacity = 25f;

        speedLevel = 0;
        pounceSpeedLevel = 0;
        clawsStrengthLevel = 0;
        oxygenCapacityLevel = 0;
        carryingCapacityLevel = 0;

        hasMiniMap = false;

        SaveUpgrade();

        Debug.Log("Player stats have been reset!");
    }
}