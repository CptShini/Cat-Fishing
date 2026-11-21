using UnityEngine;
using Assets.Scripts.Aquarium.Domain;
using System.Collections.Generic;

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

                if (slot.ItemSO != itemSO) continue;
                
                slot.count++;
                slot.UpdateUI();
                return true;
            }
        }

        DecorInventorySlot newSlot = Instantiate(slotPrefab, inventoryContent);

        newSlot.Setup(itemSO, placementController);

        return true;
    }

    public bool AddTank(ItemSO itemSO, Tank tank)
    {
        DecorInventorySlot newSlot = Instantiate(slotPrefab, inventoryContent);

        newSlot.SetupTank(itemSO, placementController, tank);

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

    public void OnSlotClicked(DecorInventorySlot slot)
    {
        placementController.SelectItem(slot);
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


    public void GetInventorySaveData(List<InventoryItemSaveData> saveData)
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
                count = slot.count,
                instanceID = slot.InstanceID,
                tankData = slot.SavedTankData
            });
        }
    }

    public void LoadInventory(List<InventoryItemSaveData> saveData, ItemDatabase database)
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

            if (savedItem.tankData != null)
            {
                newSlot.LoadTankData(savedItem.instanceID, savedItem.tankData);
            }

            newSlot.UpdateUI();
        }
    }

    // for testing
    public void ClearInventory()
    {
        foreach (Transform child in inventoryContent)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }
}