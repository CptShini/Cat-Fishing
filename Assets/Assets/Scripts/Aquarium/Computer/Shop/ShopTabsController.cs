using System.Collections.Generic;
using Assets.Scripts.Audio;
using UnityEngine;

public class ShopTabsController : MonoBehaviour
{
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopDecorItems;
    [SerializeField] private List<ShopItems> shopUpgradeItems;

    private void Start()
    {
        OpenDecorShop();
    }
    
    public void OpenDecorShop()
    {
        shopManager.PopulateShopItems(shopDecorItems);
        AudioManager.Instance.Play(AudioId.ButtonClick);
    }

    public void OpenUpgradeShop()
    {
        shopManager.PopulateShopItems(shopUpgradeItems);
        AudioManager.Instance.Play(AudioId.ButtonClick);
    }
}
