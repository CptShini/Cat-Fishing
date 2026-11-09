using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ShopItemType
{
    Upgrade,
    MiniMap
}

public class ShopUpgradeItem : MonoBehaviour
{
    [Header("Item Type")]
    [SerializeField] private ShopItemType itemType;

    [Header("Upgrade")]
    [SerializeField] private PlayerUpgradeType upgradeType;
    [SerializeField] private string displayName;

    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;

    private ShopStatsData stats;

    private void Start()
    {
        stats = FindFirstObjectByType<ShopStatsData>();

        nameText.text = displayName;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (itemType == ShopItemType.MiniMap)
        {
            levelText.gameObject.SetActive(false);
            valueText.gameObject.SetActive(false);
            nameText.text = "Mini Map";
            return;
        }

        valueText.text = stats.GetStatValue(upgradeType).ToString();

        int level = stats.GetUpgradeLevel(upgradeType);

        levelText.text = $"LEVEL {level}/5";
    }

    public void BuyUpgrade()
    {
        stats = FindFirstObjectByType<ShopStatsData>();

        if (stats == null)
            return;

        if (itemType == ShopItemType.MiniMap)
            stats.PurchaseMiniMap();
        else
            stats.UpgradeStat(upgradeType);
            
        UpdateUI();
    }
}