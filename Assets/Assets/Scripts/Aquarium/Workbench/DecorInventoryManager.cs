using TMPro;
using UnityEngine;

public class DecorInventoryManager : MonoBehaviour
{
    [SerializeField] private PlacementController placementController;
    public DecorInventorySlot[] inventorySlots;
    public int gold;
    public TMP_Text goldText;
    
    public void Start()
    {
        goldText.text = gold.ToString();
        foreach (var slot in inventorySlots)
        {
            slot.gameObject.SetActive(slot.ItemSO != null);

            if (slot.ItemSO != null && slot.count <= 0)
                slot.count = 1;

            slot.UpdateUI();
        }
    }
    
    public bool AddItem(ItemSO itemSO)
    {
        goldText.text = gold.ToString();

        // if the item is a floor tile, check if it already exists in the inventory and increment the count
        if (itemSO.itemType == ItemType.FloorTile)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.ItemSO == itemSO)
                {
                    slot.count++;
                    slot.UpdateUI();
                    return true;
                }
            }
        }

        // if the item is not a floor tile, or if it is a floor tile that doesn't exist in the inventory, add it to an empty slot
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemSO == null)
            {
                slot.ItemSO = itemSO;
                slot.count = 1;
                slot.gameObject.SetActive(true);
                slot.UpdateUI();
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(ItemSO itemSO)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemSO != itemSO)
                continue;

            // floor tiles
            if (itemSO.itemType == ItemType.FloorTile)
            {
                slot.count--;

                if (slot.count > 0)
                {
                    slot.UpdateUI();
                    return;
                }
            }

            // furniture
            slot.ItemSO = null;
            slot.count = 1;

            slot.gameObject.SetActive(false);
            slot.UpdateUI();

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
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemSO == itemSO && slot.count > 0)
                return true;
        }

        return false;
    }
}