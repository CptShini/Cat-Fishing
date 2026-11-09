using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private int gold;

    public int Gold => gold;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GoldUI.Instance?.UpdateGold(gold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || gold < amount)
            return false;

        gold -= amount;
        UpdateUI();

        return true;
    }

    public bool HasEnoughGold(int amount)
    {
        return amount >= 0 && gold >= amount;
    }

    private void UpdateUI()
    {
        GoldUI.Instance.UpdateGold(gold);
    }
}