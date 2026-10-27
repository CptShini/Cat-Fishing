using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorInventorySlot : MonoBehaviour
{
    public ItemSO ItemSO;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text name;

    public void UpdateUI()
    {
        if (ItemSO != null)
        {
            icon.sprite = ItemSO.itemIcon;
            icon.gameObject.SetActive(true);
            name.text = ItemSO.itemName;
            name.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
            name.gameObject.SetActive(false);
        }
    }
}
