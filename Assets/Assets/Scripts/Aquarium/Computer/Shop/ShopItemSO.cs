using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int itemPrice;
}