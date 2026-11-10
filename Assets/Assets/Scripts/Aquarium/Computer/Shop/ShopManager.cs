using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private ShopSlot shopSlotPrefab;
    [SerializeField] private Transform shopContent;

    [Header("Inventory")]
    [SerializeField] private DecorInventoryManager inventoryManager;

    private readonly HashSet<ItemSO> purchasedItems = new();

    private readonly List<ShopSlot> spawnedSlots = new();

    public bool IsPurchased(ItemSO itemSO)
    {
        // you can buy floor tiles multiple times
        if (itemSO == null || itemSO.itemType == ItemType.FloorTile)
            return false;

        return purchasedItems.Contains(itemSO);
    }

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        // remove any existing slots before populating new ones
        foreach (ShopSlot slot in spawnedSlots.Where(slot => slot))
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        // generate slots based on the number of ShopItems
        foreach (ShopItems shopItem in shopItems)
        {
            if (shopItem == null || shopItem.itemSO == null)
                continue;

            ShopSlot newSlot = Instantiate(shopSlotPrefab, shopContent);

            newSlot.Initialize(shopItem.itemSO, shopItem.price, this);

            spawnedSlots.Add(newSlot);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if (itemSO.itemType == ItemType.Furniture && IsPurchased(itemSO))
        {
            Debug.Log("Already purchased: " + itemSO.itemName);
            return;
        }

        if (!GoldManager.Instance.HasEnoughGold(price))
        {
            Debug.Log("Not enough gold.");
            return;
        }


        if (!GoldManager.Instance.SpendGold(price))
            return;

        inventoryManager.AddItem(itemSO);
        
        if (itemSO.itemType == ItemType.Furniture)
            purchasedItems.Add(itemSO);
        
        RefreshShopSlots();

        SaveManager.Instance.SaveData();
        SaveManager.Instance.Save();
        
        Debug.Log("Purchased: " + itemSO.itemName);
     
    }

    private void RefreshShopSlots()
    {
        foreach (ShopSlot slot in spawnedSlots.Where(slot => slot.gameObject.activeSelf))
        {
            slot.UpdatePurchasedState();
        }
    }

    // for save
    public void GetPurchasedItemsSaveData(List<string> saveData)
    {
        saveData.Clear();

        foreach (ItemSO item in purchasedItems)
        {
            if (item != null && item.itemType != ItemType.FloorTile)
                saveData.Add(item.itemID);
        }
    }

    public void LoadPurchasedItems(List<string> savedItemIDs, ItemDatabase database)
    {
        purchasedItems.Clear();

        if (savedItemIDs == null)
            return;

        foreach (string itemID in savedItemIDs)
        {
            ItemSO item = database.GetItem(itemID);

            if (item != null && item.itemType != ItemType.FloorTile)
                purchasedItems.Add(item);
        }

        foreach (ShopSlot slot in spawnedSlots)
        {
            if (slot != null)
                slot.UpdatePurchasedState();
        }
    }

    // for testing
    public void ResetPurchasedItems()
    {
        purchasedItems.Clear();
        RefreshShopSlots();
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}