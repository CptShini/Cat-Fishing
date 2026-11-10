using UnityEngine;

public class DecorInventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private DecorInventorySlot slotPrefab;
    [SerializeField] private Transform inventoryContent;

    [Header("Placement")]
    [SerializeField] private PlacementController placementController;
    
    
    public bool AddItem(ItemSO itemSO)
    {
        // if the item is a floor tile, check if it already exists in the inventory and increment the count
        if (itemSO.itemType == ItemType.FloorTile)
        {
            foreach (Transform child in inventoryContent)
            {
                DecorInventorySlot slot = child.GetComponent<DecorInventorySlot>();

                if (slot.ItemSO == itemSO)
                {
                    slot.count++;
                    slot.UpdateUI();
                    return true;
                }
            }
        }

        DecorInventorySlot newSlot = Instantiate(slotPrefab, inventoryContent);

        newSlot.Setup(itemSO, placementController);

        return true;
    }

    public void RemoveItem(ItemSO itemSO)
    {
        foreach (Transform child in inventoryContent)
        {
            DecorInventorySlot slot = child.GetComponent<DecorInventorySlot>();

            if (slot == null || slot.ItemSO != itemSO)
                continue;

            if (itemSO.itemType == ItemType.FloorTile)
            {
                slot.count--;

                if (slot.count > 0)
                {
                    slot.UpdateUI();
                    return;
                }
            }

            Destroy(slot.gameObject);

            return;
        }
    }

    public void OnSlotClicked(ItemSO ItemSO)
    {
        placementController.SelectItem(ItemSO);
    }

    // check if the inventory has a specific item
    public bool HasItem(ItemSO itemSO)
    {
        foreach (Transform child in inventoryContent)
        {
            DecorInventorySlot slot =child.GetComponent<DecorInventorySlot>();
            if (slot.ItemSO == itemSO && slot.count > 0)
                return true;
        }

        return false;
    }


    public void GetInventorySaveData(System.Collections.Generic.List<InventoryItemSaveData> saveData)
    {
        saveData.Clear();

        foreach (Transform child in inventoryContent)
        {
            DecorInventorySlot slot = child.GetComponent<DecorInventorySlot>();

            if (slot == null || slot.ItemSO == null)
                continue;

            saveData.Add(new InventoryItemSaveData
            {
                itemID = slot.ItemSO.itemID,
                count = slot.count
            });
        }
    }

    public void LoadInventory(System.Collections.Generic.List<InventoryItemSaveData> saveData, ItemDatabase database)
    {
        foreach (Transform child in inventoryContent)
            Destroy(child.gameObject);

        foreach (InventoryItemSaveData savedItem in saveData)
        {
            ItemSO item = database.GetItem(savedItem.itemID);

            if (item == null)
                continue;

            DecorInventorySlot newSlot = Instantiate(slotPrefab, inventoryContent);

            newSlot.Setup(item, placementController);
            newSlot.count = savedItem.count;
            newSlot.UpdateUI();
        }
    }
}