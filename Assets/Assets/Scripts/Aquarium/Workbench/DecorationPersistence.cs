using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Aquarium.FishTanks;
using Assets.Scripts.Aquarium.Domain;

public class DecorationPersistence : MonoBehaviour, IDataPersistence
{
    [Header("References")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private DecorInventoryManager inventoryManager;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private RoomGrid roomGrid;

    public void LoadData(GameData data)
    {
        if (data.Decoration == null)
            data.Decoration = new DecorationSaveData();

        if (data.Tanks == null)
            data.Tanks = new();

        placementController.LoadPlacedFurniture(data.Decoration.placedFurniture, itemDatabase, data);
        placementController.LoadFixedFurniture(data.Decoration.fixedFurniture);

        inventoryManager.LoadInventory(data.Decoration.inventory, itemDatabase);

        shopManager.LoadPurchasedItems(data.Decoration.purchasedItemIDs, itemDatabase);

        roomGrid.LoadFloor(data.Decoration.baseFloor, data.Decoration.additionalFloor, itemDatabase);
    }

    public void SaveData(ref GameData data)
    {
        if (data.Decoration == null)
            data.Decoration = new DecorationSaveData();

        if (data.Tanks == null)
            data.Tanks = new();

        placementController.GetPlacedFurnitureSaveData(data.Decoration.placedFurniture);
        placementController.GetFixedFurnitureSaveData(data.Decoration.fixedFurniture);

        inventoryManager.GetInventorySaveData(data.Decoration.inventory);

        shopManager.GetPurchasedItemsSaveData(data.Decoration.purchasedItemIDs);

        roomGrid.GetFloorSaveData(data.Decoration.baseFloor, data.Decoration.additionalFloor, itemDatabase);

        data.Tanks.Clear();

        FishTank[] fishTanks = FindObjectsByType<FishTank>(FindObjectsSortMode.None);

        foreach (FishTank fishTank in fishTanks)
        {
            if (fishTank.Tank == null)
                continue;

            string tankId = fishTank.Tank.Id.ToString();

            data.Tanks[tankId] = new TankDTO(fishTank.Tank);
        }
    }
}
