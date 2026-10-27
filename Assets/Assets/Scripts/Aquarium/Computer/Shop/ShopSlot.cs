using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ShopItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;

    private int price;

    public void Initialize(ShopItemSO newShopItem, int price)
    {
        itemSO = newShopItem;
        itemNameText.text = itemSO.itemName;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        priceText.text = price.ToString();
    }

    public void OnBuyButtonClicked()
    {
        //shopManager.TryBuyItem(itemSO, price);
    }
}
