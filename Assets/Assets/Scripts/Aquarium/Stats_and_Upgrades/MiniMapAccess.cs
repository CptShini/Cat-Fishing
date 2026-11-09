using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

public class MiniMapAccess : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject canvasMiniMap;

    public void LoadData(GameData data)
    {
        bool isPurchased = false;

        if (data.PlayerStats != null && data.PlayerStats.ContainsKey("HasMiniMap"))
        {
            isPurchased = data.PlayerStats["HasMiniMap"] >= 1f;
        }
        canvasMiniMap.SetActive(isPurchased);
    }

    public void SaveData(ref GameData data)
    {
        
    }
}