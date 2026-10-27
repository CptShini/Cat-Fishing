using TMPro;
using UnityEngine;

public class DecorInventoryManager : MonoBehaviour
{
    public DecorInventorySlot[] decorInventorySlots;
    public int gold;
    public TMP_Text goldText;
    
    public void Start()
    {
        goldText.text = gold.ToString();
        foreach (var slot in decorInventorySlots)
        {
            slot.UpdateUI();
        }
    }
    
    public void AddItem(DecorItemSO itemSO)
    {
        goldText.text = gold.ToString();
        foreach (var slot in decorInventorySlots)
        {
            if (slot.decorItemSO == null)
            {
                slot.decorItemSO = itemSO;
                slot.UpdateUI();
                return;
            }
        }
    }
}
