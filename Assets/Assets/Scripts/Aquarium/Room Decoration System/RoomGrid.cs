using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap placeableArea;
    [SerializeField] private DecorationGridVisuals decorationGridVisuals;

    [Header("Collision")]
    [SerializeField] private LayerMask placementBlockerMask;

    [Header("Floor Bottom Edge")]
    [SerializeField] private Transform floorEdgeRoot;
    [SerializeField] private Sprite bottomEdgeSprite;
    [SerializeField] private Vector2 bottomEdgeOffset;
    [SerializeField] private int bottomEdgeSortingOrder = 0;

    private Dictionary<Vector3Int, GameObject> bottomEdges = new();

    private HashSet<Vector3Int> occupiedCells = new();

    private void Start()
    {
        RefreshBottomEdges();
    }

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

        RefreshBottomEdges();
        decorationGridVisuals.RefreshGrid();

        return true;
    }

    // snaps the origin of a floor tile placement to a 3x3 grid
    public Vector3Int SnapFloorOrigin(Vector3Int cell)
    {
        int x = Mathf.FloorToInt(cell.x / 3f) * 3;
        int y = Mathf.FloorToInt(cell.y / 3f) * 3;

        return new Vector3Int(x, y, cell.z);
    }

    public void RefreshBottomEdges()
    {
        if (placeableArea == null || bottomEdgeSprite == null)
            return;

        HashSet<Vector3Int> requiredEdges = new HashSet<Vector3Int>();

        BoundsInt bounds = placeableArea.cellBounds;

        // bottom edges
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!placeableArea.HasTile(cell))
                continue;

            Vector3Int cellBelow = cell + Vector3Int.down;

            if (!placeableArea.HasTile(cellBelow))
            {
                requiredEdges.Add(cell);
            }
        }

        // deleting old bottom edge
        List<Vector3Int> edgesToRemove = new List<Vector3Int>();

        foreach (var edge in bottomEdges)
        {
            if (!requiredEdges.Contains(edge.Key))
            {
                if (edge.Value != null)
                    Destroy(edge.Value);

                edgesToRemove.Add(edge.Key);
            }
        }

        foreach (Vector3Int cell in edgesToRemove)
        {
            bottomEdges.Remove(cell);
        }

        // creating new bottom edges
        foreach (Vector3Int cell in requiredEdges)
        {
            if (bottomEdges.ContainsKey(cell))
                continue;

            GameObject edgeObject = new GameObject("FloorBottomEdge");

            edgeObject.transform.SetParent(floorEdgeRoot);

            edgeObject.transform.position = grid.GetCellCenterWorld(cell) + (Vector3)bottomEdgeOffset;

            SpriteRenderer renderer = edgeObject.AddComponent<SpriteRenderer>();

            renderer.sprite = bottomEdgeSprite;
            renderer.sortingLayerName = "AboveWorld";
            renderer.sortingOrder = bottomEdgeSortingOrder;

            bottomEdges.Add(cell, edgeObject);
        }
    }
}