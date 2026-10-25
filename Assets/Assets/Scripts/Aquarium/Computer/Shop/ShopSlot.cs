using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ShopItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    private int price;

    public void Initialize(ShopItemSO newShopItem, int price)
    {
        itemSO = newShopItem;
        itemNameText.text = itemSO.itemName;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        priceText.text = price.ToString();
    }
}
