using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;
    public Image SoldIndicator;
    
    private ShopManager shopManager;

    private int price;

    public void Initialize(ItemSO newItemSO, int price, ShopManager shopManager)
    {
        itemSO = newItemSO;
        itemNameText.text = itemSO.itemName;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        this.shopManager = shopManager;
        priceText.text = price.ToString();

        UpdatePurchasedState();
    }

    public void UpdatePurchasedState()
    {
        bool isSold = shopManager.IsPurchased(itemSO);
        SoldIndicator.gameObject.SetActive(isSold);
    }

    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSO, price);
    }
}
