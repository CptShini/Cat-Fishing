using UnityEngine;

public class RoomFixedFurniture : MonoBehaviour
{
    [Header("Grid")]
    public Vector2Int size = Vector2Int.one;

    [Header("Placement")]
    public bool blocksPlacement = true;

    [HideInInspector] public Vector3Int currentOrigin;

    public Vector3Int GetOrigin(Grid grid)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            return grid.WorldToCell(transform.position);

        // left bottom corner of the sprite bounds
        Vector3 bottomLeft = spriteRenderer.bounds.min;

        return grid.WorldToCell(bottomLeft);
    }
}
