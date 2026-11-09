using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class ShopStatsData : MonoBehaviour, IDataPersistence
{
    [Header("Player Stats")]
    public float speed = 10f;
    public float pounceSpeed = 20f;
    public float clawsStrength = 10f;
    public float oxygenCapacity = 100f;
    public float carryingCapacity = 25f;

    [Header("Upgrade Amounts")]
    [SerializeField] private float speedUpgrade = 2f;
    [SerializeField] private float pounceSpeedUpgrade = 2f;
    [SerializeField] private float clawsStrengthUpgrade = 2f;
    [SerializeField] private float oxygenUpgrade = 20f;
    [SerializeField] private float carryingCapacityUpgrade = 5f;

    public void LoadData(GameData data)
    {
        if (data.PlayerStats == null)
            data.PlayerStats = new();

        speed = GetStat(data, "Speed", 10f);
        pounceSpeed = GetStat(data, "PounceSpeed", 20f);
        clawsStrength = GetStat(data, "ClawsStrength", 10f);
        oxygenCapacity = GetStat(data, "OxygenCapacity", 100f);
        carryingCapacity = GetStat(data, "CarryingCapacity", 25f);
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
    }

    private float GetStat(GameData data, string key, float defaultValue)
    {
        if (!data.PlayerStats.ContainsKey(key))
        {
            data.PlayerStats[key] = defaultValue;
        }

        return data.PlayerStats[key];
    }


    public void UpgradeSpeed()
    {
        speed += speedUpgrade;
        SaveUpgrade();
    }

    public void UpgradePounceSpeed()
    {
        pounceSpeed += pounceSpeedUpgrade;
        SaveUpgrade();
    }

    public void UpgradeClawsStrength()
    {
        clawsStrength += clawsStrengthUpgrade;
        SaveUpgrade();
    }

    public void UpgradeOxygen()
    {
        oxygenCapacity += oxygenUpgrade;
        SaveUpgrade();
    }

    public void UpgradeCarryingCapacity()
    {
        carryingCapacity += carryingCapacityUpgrade;
        SaveUpgrade();
    }


    private void SaveUpgrade()
    {
        SaveManager.Instance.SaveData();
        SaveManager.Instance.Save();
    }


    // just to test
    public void ResetStats()
    {
        speed = 10f;
        pounceSpeed = 20f;
        clawsStrength = 10f;
        oxygenCapacity = 100f;
        carryingCapacity = 25f;

        SaveUpgrade();

        Debug.Log("Player stats have been reset!");
    }
}