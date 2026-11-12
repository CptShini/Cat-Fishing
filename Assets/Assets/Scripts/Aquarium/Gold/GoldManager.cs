using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class GoldManager : MonoBehaviour, IDataPersistence
{
    public static GoldManager Instance { get; private set; }

    private int _gold;

    public int Gold
    {
        get => _gold;
        private set
        {
            _gold = value;
            UpdateUI();
        }
    }

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
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount)
            return false;

        Gold -= amount;

        return true;
    }

    public bool HasEnoughGold(int amount)
    {
        return amount >= 0 && Gold >= amount;
    }

    private void UpdateUI()
    {
        GoldUI.Instance.UpdateGold(Gold);
    }

    public void LoadData(GameData data)
    {
        Gold = data.Gold;
    }

    public void SaveData(ref GameData data)
    {
        data.Gold = Gold;
    }

    // for testing
    public void ResetGold()
    {
        Gold = 500;
        UpdateUI();

        Debug.Log("Gold reset to 500.");
    }
}