using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;
    public Image WaterIndicator;

    [SerializeField] private ShopManager shopManager;

    private int price;

    public void Initialize(ItemSO newItemSO, int price)
    {
        itemSO = newItemSO;
        itemNameText.text = itemSO.itemName;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        priceText.text = price.ToString();

        bool isTank = itemSO.itemType == ItemType.TankDecoration;
        WaterIndicator.gameObject.SetActive(isTank);
    }

    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSO, price);
    }
}
