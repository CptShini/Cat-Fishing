using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Upgrades;

public class ShopStatsData : MonoBehaviour, IDataPersistence
{
    public bool hasMinimap { get; private set; }

    [field: SerializeField] public UpgradeLedger UpgradeLedger { get; private set; }

    private int 
        _speedLevel,
        _pounceSpeedLevel,
        _clawStrengthLevel,
        _oxygenCapacityLevel,
        _carryingCapacityLevel;

    public float GetStatValue(LevelUpgrade type, int level) =>
        UpgradeLedger.GetUpgradeLevels(type)[level].Stat;

    public int GetUpgradeLevel(LevelUpgrade type) =>
        type switch
        {
            LevelUpgrade.Speed => _speedLevel,
            LevelUpgrade.PounceSpeed => _pounceSpeedLevel,
            LevelUpgrade.ClawStrength => _clawStrengthLevel,
            LevelUpgrade.OxygenCapacity => _oxygenCapacityLevel,
            LevelUpgrade.CarryingCapacity => _carryingCapacityLevel,
            _ => 0
        };

    public void UpgradeStat(LevelUpgrade type)
    {
        if (GetUpgradeLevel(type) >= UpgradeLedger.GetLevelCount(type))
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
            case LevelUpgrade.Speed:
                _speedLevel++;
                break;
            case LevelUpgrade.PounceSpeed:
                _pounceSpeedLevel++;
                break;
            case LevelUpgrade.ClawStrength:
                _clawStrengthLevel++;
                break;
            case LevelUpgrade.OxygenCapacity:
                _oxygenCapacityLevel++;
                break;
            case LevelUpgrade.CarryingCapacity:
                _carryingCapacityLevel++;
                break;
            default:
                return;
        }
    }

    public void PurchaseMiniMap()
    {
        if (hasMinimap)
        {
            Debug.Log("Minimap is already purchased");
            return;
        }

        if (!GoldManager.Instance.SpendGold(UpgradeLedger.MinimapPrice))
        {
            Debug.Log("Not enough gold to purchase MiniMap!");
            return;
        }

        hasMinimap = true;

        Debug.Log("Minimap purchased");
    }

    public int GetUpgradeCost(LevelUpgrade type)
    {
        int level = GetUpgradeLevel(type);
        UpgradeLevel[] upgradeLevels = UpgradeLedger.GetUpgradeLevels(type);
        return upgradeLevels[level].Price;
    }

    public void LoadData(GameData data)
    {
        _speedLevel = data.LevelUpgrades[LevelUpgrade.Speed];
        _pounceSpeedLevel = data.LevelUpgrades[LevelUpgrade.PounceSpeed];
        _clawStrengthLevel = data.LevelUpgrades[LevelUpgrade.ClawStrength];
        _oxygenCapacityLevel = data.LevelUpgrades[LevelUpgrade.OxygenCapacity];
        _carryingCapacityLevel = data.LevelUpgrades[LevelUpgrade.CarryingCapacity];

        hasMinimap = data.OneTimeUpgrades[OneTimeUpgrade.Minimap];
    }

    public void SaveData(ref GameData data)
    {
        data.LevelUpgrades[LevelUpgrade.Speed] = _speedLevel;
        data.LevelUpgrades[LevelUpgrade.PounceSpeed] = _pounceSpeedLevel;
        data.LevelUpgrades[LevelUpgrade.ClawStrength] = _clawStrengthLevel;
        data.LevelUpgrades[LevelUpgrade.OxygenCapacity] = _oxygenCapacityLevel;
        data.LevelUpgrades[LevelUpgrade.CarryingCapacity] = _carryingCapacityLevel;

        data.OneTimeUpgrades[OneTimeUpgrade.Minimap] = hasMinimap;
    }
    
    public void ResetStats()
    {
        _speedLevel = 0;
        _pounceSpeedLevel = 0;
        _clawStrengthLevel = 0;
        _oxygenCapacityLevel = 0;
        _carryingCapacityLevel = 0;

        hasMinimap = false;
    }
}