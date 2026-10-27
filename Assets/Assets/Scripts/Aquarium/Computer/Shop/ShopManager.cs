using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItems> shopItems;

    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private DecorInventoryManager inventoryManager;

    private void Start()
    {
        PopulateShopItems();
    }

    public void PopulateShopItems()
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
        if(itemSO != null && inventoryManager.gold >= price)
        {
            if(HasSpaceForItem(itemSO))
            {
                inventoryManager.gold -= price;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO);
            }
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if(slot.ItemSO == itemSO)
                return true;
            else if(slot.ItemSO == null)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}