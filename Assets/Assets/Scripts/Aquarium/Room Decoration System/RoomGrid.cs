using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap placeableArea;

    [Header("Collision")]
    [SerializeField] private LayerMask placementBlockerMask;

    private HashSet<Vector3Int> occupiedCells = new();

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 GetPlacementPosition(Vector3Int origin, Vector2Int size)
    {
        Vector3 bottomLeft = grid.CellToWorld(origin);

        Vector3 offset = new Vector3(size.x * grid.cellSize.x / 2f, size.y * grid.cellSize.y / 2f, 0f);

        return bottomLeft + offset;
    }

    public bool CanPlace(Vector3Int origin, Vector2Int size, bool blocksPlacement)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);

                if (placeableArea != null && !placeableArea.HasTile(cell))
                {
                    return false;
                }

                if (blocksPlacement && occupiedCells.Contains(cell))
                {
                    return false;
                }

                if (IsCellBlocked(cell))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsCellBlocked(Vector3Int cell)
    {
        Vector3 center = grid.GetCellCenterWorld(cell);

        Vector2 size = new Vector2(grid.cellSize.x * 0.9f, grid.cellSize.y * 0.9f);

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, placementBlockerMask);

        return hit != null;
    }

    public void RegisterObject(Vector3Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                occupiedCells.Add(origin + new Vector3Int(x, y, 0));
            }
        }
    }

    public void UnregisterObject(Vector3Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                occupiedCells.Remove(origin + new Vector3Int(x, y, 0));
            }
        }
    }
}