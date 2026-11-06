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

    public bool CanExpandRoom(Vector3Int origin, Vector2Int size)
    {
        if (placeableArea == null)
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);

                if (placeableArea.HasTile(cell))
                    return false;

                if (occupiedCells.Contains(cell))
                    return false;

                if (IsCellBlocked(cell))
                    return false;
            }
        }

        return HasAdjacentFloor(origin, size);
    }

    // area must be adjacent to existing floor tiles
    private bool HasAdjacentFloor(Vector3Int origin, Vector2Int size)
    {
        // upper and lower edge
        for (int x = 0; x < size.x; x++)
        {
            Vector3Int bottom = origin + new Vector3Int(x, -1, 0);
            Vector3Int top = origin + new Vector3Int(x, size.y, 0);

            if (placeableArea.HasTile(bottom) || placeableArea.HasTile(top))
                return true;
        }

        // left and right edge
        for (int y = 0; y < size.y; y++)
        {
            Vector3Int left = origin + new Vector3Int(-1, y, 0);
            Vector3Int right = origin + new Vector3Int(size.x, y, 0);

            if (placeableArea.HasTile(left) || placeableArea.HasTile(right))
                return true;
        }

        return false;
    }

    public bool ExpandRoom(Vector3Int origin, Vector2Int size, TileBase floorTile)
    {
        if (floorTile == null || !CanExpandRoom(origin, size))
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);
                placeableArea.SetTile(cell, floorTile);
            }
        }

        return true;
    }
}