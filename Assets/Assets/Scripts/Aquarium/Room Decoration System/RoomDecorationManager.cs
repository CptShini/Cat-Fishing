using UnityEngine;

public class RoomDecorationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomGrid roomGrid;
    [SerializeField] private Transform fixedFurnitureRoot;
    [SerializeField] private Grid grid;
    [SerializeField] private DecorationGridVisuals gridVisuals;
    [SerializeField] private PlacementController placementController;

    private bool isDecorationMode;

    private void Awake()
    {
        RegisterFixedFurniture();
    }

    private void RegisterFixedFurniture()
    {
        if (fixedFurnitureRoot == null)
            return;

        RoomFixedFurniture[] furniture = fixedFurnitureRoot.GetComponentsInChildren<RoomFixedFurniture>();

        foreach (RoomFixedFurniture item in furniture)
        {
            Vector3Int origin = item.GetOrigin(grid);
            gridVisuals.RegisterFixedFurniture(item, origin);

            if (item.blocksPlacement)
                roomGrid.RegisterObject(origin, item.size);
        }
    }

    public void EnterDecorationMode()
    {
        if (isDecorationMode)
            return;

        isDecorationMode = true;

        placementController.EnterDecorationMode();

        gridVisuals.ShowGrid();
    }

    public void ExitDecorationMode()
    {
        if (!isDecorationMode)
            return;

        isDecorationMode = false;

        placementController.ExitDecorationMode();

        gridVisuals.HideGrid();
        gridVisuals.HideFootprint();
    }
}