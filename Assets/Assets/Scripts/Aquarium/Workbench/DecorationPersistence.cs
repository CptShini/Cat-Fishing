
using UnityEngine;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;

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

        placementController.LoadPlacedFurniture(data.Decoration.placedFurniture, itemDatabase);
        placementController.LoadFixedFurniture(data.Decoration.fixedFurniture);

        inventoryManager.LoadInventory(data.Decoration.inventory, itemDatabase);

        shopManager.LoadPurchasedItems(data.Decoration.purchasedItemIDs, itemDatabase);

        roomGrid.LoadPlacedFloor(data.Decoration.placedFloor, itemDatabase);
    }

    public void SaveData(ref GameData data)
    {
        if (data.Decoration == null)
            data.Decoration = new DecorationSaveData();

        placementController.GetPlacedFurnitureSaveData(data.Decoration.placedFurniture);
        placementController.GetFixedFurnitureSaveData(data.Decoration.fixedFurniture);

        inventoryManager.GetInventorySaveData(data.Decoration.inventory);

        shopManager.GetPurchasedItemsSaveData(data.Decoration.purchasedItemIDs);

        roomGrid.GetPlacedFloorSaveData( data.Decoration.placedFloor, itemDatabase);
    }
}
