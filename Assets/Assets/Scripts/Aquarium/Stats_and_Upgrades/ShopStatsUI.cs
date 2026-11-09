using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopStatsUI : MonoBehaviour
{
    [SerializeField] private ShopStatsData stats;

    [SerializeField] private TMP_Text miniMapStatusText;

    private void Start()
    {
        FindStats();
        UpdateMiniMapUI();
    }

    private void FindStats()
    {
        if (stats == null)
            stats = FindFirstObjectByType<ShopStatsData>();
    }

    public void UpdateMiniMapUI()
    {
        FindStats();

        miniMapStatusText.text = stats.hasMiniMap
            ? "PURCHASED"
            : "NOT PURCHASED";
    }

    public void PurchaseMiniMap()
    {
        FindStats();

        stats.PurchaseMiniMap();
        UpdateMiniMapUI();
    }

    // to test
    public void ResetStats()
    {
        stats.ResetStats();
        UpdateMiniMapUI();

        ShopUpgradeItem[] items = FindObjectsByType<ShopUpgradeItem>(FindObjectsSortMode.None);

        foreach (ShopUpgradeItem item in items)
        {
            item.UpdateUI();
        }
    }
}