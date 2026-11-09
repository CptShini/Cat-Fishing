using UnityEngine;
using TMPro;

public class ShopStatsUI : MonoBehaviour
{
    [Header("Stats Data")]
    [SerializeField] private ShopStatsData stats;

    [Header("UI")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text pounceSpeedText;
    [SerializeField] private TMP_Text clawsStrengthText;
    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private TMP_Text carryingCapacityText;

    private void Start()
    {
        if (stats == null)
            stats = FindFirstObjectByType<ShopStatsData>();

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (stats == null) return;

        speedText.text = stats.speed.ToString();
        pounceSpeedText.text = stats.pounceSpeed.ToString();
        clawsStrengthText.text = stats.clawsStrength.ToString();
        oxygenText.text = stats.oxygenCapacity.ToString();
        carryingCapacityText.text = stats.carryingCapacity.ToString();
    }

    public void UpgradeSpeed()
    {
        stats.UpgradeSpeed();
        UpdateUI();
    }

    public void UpgradePounceSpeed()
    {
        stats.UpgradePounceSpeed();
        UpdateUI();
    }

    public void UpgradeClawsStrength()
    {
        stats.UpgradeClawsStrength();
        UpdateUI();
    }

    public void UpgradeOxygen()
    {
        stats.UpgradeOxygen();
        UpdateUI();
    }

    public void UpgradeCarryingCapacity()
    {
        stats.UpgradeCarryingCapacity();
        UpdateUI();
    }

    // to test
    public void ResetStats()
    {
        stats.ResetStats();
        UpdateUI();
    }
}