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
            slot.UpdateUI();
        }
    }
    
    public bool AddItem(ItemSO itemSO)
    {
        if (itemSO == null)
            return false;

        goldText.text = gold.ToString();
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemSO == null)
            {
                slot.ItemSO = itemSO;
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
            if (slot.ItemSO == itemSO)
            {
                slot.ItemSO = null;
                slot.gameObject.SetActive(false);
                slot.UpdateUI();
                return;
            }
        }
    }

    public void OnSlotClicked(ItemSO ItemSO)
    {
        if (ItemSO == null)
            return;

        placementController.SelectItem(ItemSO);
    }
}