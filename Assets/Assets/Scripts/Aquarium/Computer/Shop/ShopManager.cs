using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        if (inventoryManager.gold < price)
        {
            Debug.Log("Not enough gold.");
            return;
        }

        if(itemSO != null && inventoryManager.gold >= price)
        {
            /*if(HasSpaceForItem(itemSO))
            {*/
                inventoryManager.gold -= price;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO);
                if (itemSO.itemType == ItemType.Furniture)
                    purchasedItems.Add(itemSO);
                RefreshShopSlots();
                Debug.Log("Purchased: " + itemSO.itemName);
            /*}
            else
            {
                Debug.Log("No space in inventory.");
                return;
            }*/
        }
    }

    /*private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if(slot.ItemSO == itemSO)
                return true;
            else if(slot.ItemSO == null)
                return true;
        }
        return false;
    }*/

    private void RefreshShopSlots()
    {
        foreach (ShopSlot slot in spawnedSlots.Where(slot => slot.gameObject.activeSelf))
        {
            slot.UpdatePurchasedState();
        }
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}