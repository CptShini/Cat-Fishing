using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorInventorySlot : MonoBehaviour
{
    public ItemSO ItemSO;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;

    [SerializeField] private PlacementController placementController;

    public int count = 1;

    public void OnSlotClicked()
    {
        if (ItemSO == null)
            return;

        placementController.SelectItem(ItemSO);
    }
    
    public void UpdateUI()
    {
        if (ItemSO != null)
        {
            icon.sprite = ItemSO.itemIcon;
            icon.gameObject.SetActive(true);
            nameText.text = ItemSO.itemName;
            nameText.gameObject.SetActive(true);

            // count counter for floor tiles, hidden for other items
            bool showCount = ItemSO.itemType == ItemType.FloorTile;

            countText.gameObject.SetActive(showCount);

            if (showCount)
                countText.text = "x" + count;
        }
        else
        {
            icon.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }
    }
}