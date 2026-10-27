using TMPro;
using UnityEngine;

public class DecorInventoryManager : MonoBehaviour
{
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
    
    public void AddItem(ItemSO itemSO)
    {
        goldText.text = gold.ToString();
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemSO == null)
            {
                slot.ItemSO = itemSO;
                slot.gameObject.SetActive(true);
                slot.UpdateUI();
                return;
            }
        }
    }
}
