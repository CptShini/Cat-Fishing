using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private TMP_Text miniMapStatusText;

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

        if (miniMapStatusText != null)
        {
            miniMapStatusText.text = stats.hasMiniMap
                ? "PURCHASED"
                : "NOT PURCHASED";
        }
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

    public void PurchaseMiniMap()
    {
        if (stats == null)
        stats = FindFirstObjectByType<ShopStatsData>();

        if (stats == null)
            return;

        stats.PurchaseMiniMap();
        UpdateUI();
    }

    // to test
    public void ResetStats()
    {
        stats.ResetStats();
        UpdateUI();
    }
}