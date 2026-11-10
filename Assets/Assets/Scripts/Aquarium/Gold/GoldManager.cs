using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class GoldManager : MonoBehaviour, IDataPersistence
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
        SaveGold();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || gold < amount)
            return false;

        gold -= amount;
        
        UpdateUI();
        SaveGold();

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

    private void SaveGold()
    {
        SaveManager.Instance.SaveData();
        SaveManager.Instance.Save();
    }

    public void LoadData(GameData data)
    {
        gold = data.Gold;
        UpdateUI();
    }

    public void SaveData(ref GameData data)
    {
        data.Gold = gold;
    }

    // for testing
    public void ResetGold()
    {
        gold = 500;
        UpdateUI();
        SaveGold();

        Debug.Log("Gold reset to 500.");
    }
}