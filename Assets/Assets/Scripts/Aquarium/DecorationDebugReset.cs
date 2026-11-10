using UnityEngine;

public class DecorationDebugReset : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DecorInventoryManager inventoryManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private RoomGrid roomGrid;

    public void ResetDecorationSave()
    {
        placementController.ResetFurniture();
        inventoryManager.ClearInventory();
        shopManager.ResetPurchasedItems();
        roomGrid.RestoreInitialFloor();

        Debug.Log("Decoration save reset");
    }
}
