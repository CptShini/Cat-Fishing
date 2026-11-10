using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Upgrades;

public class MiniMapAccess : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject canvasMiniMap;

    public void LoadData(GameData data)
    {
        bool isPurchased = false;
        if (data.OneTimeUpgrades.TryGetValue(OneTimeUpgrade.Minimap, out bool hasMinimap))
        {
            isPurchased = hasMinimap;
        }
        
        canvasMiniMap.SetActive(isPurchased);
    }

    public void SaveData(ref GameData data)
    {
        // Nothing to save
    }
}