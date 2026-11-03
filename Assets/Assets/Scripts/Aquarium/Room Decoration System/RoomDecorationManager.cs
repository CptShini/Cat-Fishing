using UnityEngine;

public class RoomDecorationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomGrid roomGrid;
    [SerializeField] private Transform fixedFurnitureRoot;
    [SerializeField] private Grid grid;

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
            if (!item.blocksPlacement)
                continue;

            Vector3Int origin = item.GetOrigin(grid);

            roomGrid.RegisterObject(origin, item.size);
        }
    }
}