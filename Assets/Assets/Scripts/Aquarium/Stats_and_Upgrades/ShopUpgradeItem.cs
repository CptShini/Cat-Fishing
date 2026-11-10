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
    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private GameObject slotVisual;
    [SerializeField] private GameObject buyButton;

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

            if (stats.hasMiniMap)
            {
                slotVisual.SetActive(true);
                buyButton.SetActive(false);
            }
            else
            {
                buyButtonText.text = $"{stats.GetMiniMapPrice()} GOLD";

                slotVisual.SetActive(false);
                buyButton.SetActive(true);
            }

            return;
        }

        valueText.text = stats.GetStatValue(upgradeType).ToString();

        int level = stats.GetUpgradeLevel(upgradeType);

        bool isMaxLevel = level >= 5;

        if (isMaxLevel)
        {
            levelText.text = "MAX LEVEL";

            slotVisual.SetActive(true);
            buyButton.SetActive(false);
        }
        else
        {
            levelText.text = $"Lvl {level} → Lvl {level + 1}";
            buyButtonText.text = $"{stats.GetUpgradeCost(upgradeType)} GOLD";

            slotVisual.SetActive(false);
            buyButton.SetActive(true);
        }
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