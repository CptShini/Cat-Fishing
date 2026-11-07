using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private DecorInventoryManager inventoryManager;

    private readonly HashSet<ItemSO> purchasedItems = new();

    public bool IsPurchased(ItemSO itemSO)
    {
        // you can buy floor tiles multiple times
        if (itemSO == null || itemSO.itemType == ItemType.FloorTile)
            return false;

        return purchasedItems.Contains(itemSO);
    }

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
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
        foreach (ShopSlot slot in shopSlots)
        {
            if (slot.gameObject.activeSelf)
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