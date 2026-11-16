using System.Collections.Generic;
using Assets.Scripts.Audio;
using Assets.Scripts.Audio.SFX;
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
        SFXManager.Instance.Play(SFXId.ButtonClick);
    }

    public void OpenUpgradeShop()
    {
        shopManager.PopulateShopItems(shopUpgradeItems);
        SFXManager.Instance.Play(SFXId.ButtonClick);
    }
}
