using UnityEngine;

public class RoomDecorationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomGrid roomGrid;
    [SerializeField] private Transform fixedFurnitureRoot;
    [SerializeField] private Grid grid;
    [SerializeField] private DecorationGridVisuals gridVisuals;

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

            if (!item.blocksPlacement)
                continue;

            roomGrid.RegisterObject(origin, item.size);
        }
    }
}