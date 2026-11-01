using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorInventorySlot : MonoBehaviour
{
    public ItemSO ItemSO;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    public void UpdateUI()
    {
        if (ItemSO != null)
        {
            icon.sprite = ItemSO.itemIcon;
            icon.gameObject.SetActive(true);
            nameText.text = ItemSO.itemName;
            nameText.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
        }
    }
}
