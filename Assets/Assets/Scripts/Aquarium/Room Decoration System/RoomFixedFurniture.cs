using UnityEngine;

public class RoomFixedFurniture : MonoBehaviour
{
    [Header("Grid")]
    public Vector2Int size = Vector2Int.one;

    [Tooltip("Point in central left-bottom field.")]
    public Transform gridAnchor;

    [Header("Placement")]
    public bool blocksPlacement = true;

    public Vector3Int GetOrigin(Grid grid)
    {
        Transform anchor = gridAnchor != null ? gridAnchor : transform;

        return grid.WorldToCell(anchor.position);
    }
}
