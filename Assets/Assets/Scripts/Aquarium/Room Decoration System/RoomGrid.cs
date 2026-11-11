using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap placeableArea;
    [SerializeField] private DecorationGridVisuals decorationGridVisuals;
    [SerializeField] private TileBase voidTile;

    [Header("Collision")]
    [SerializeField] private LayerMask placementBlockerMask;

    [Header("Floor Bottom Edge")]
    [SerializeField] private Transform floorEdgeRoot;
    [SerializeField] private Sprite bottomEdgeSprite;
    [SerializeField] private Vector2 bottomEdgeOffset;
    [SerializeField] private int bottomEdgeSortingOrder = 0;

    private BoundsInt initialFloorBounds;
    private TileBase[] initialFloorTiles;

    private Dictionary<Vector3Int, GameObject> bottomEdges = new();

    private HashSet<Vector3Int> occupiedCells = new();

    public Grid Grid => grid;

    private void Awake()
    {
        if (placeableArea == null)
            return;

        initialFloorBounds = placeableArea.cellBounds;
        initialFloorTiles = placeableArea.GetTilesBlock(initialFloorBounds);
    }

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
        {
            return false;
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);
                
                if (placeableArea.HasTile(cell))
                {
                    TileBase tile = placeableArea.GetTile(cell);

                    if(tile == voidTile)
                    {
                        continue;
                    }
                    return false;
                }

                if (occupiedCells.Contains(cell))
                {
                    return false;
                }

                if (IsCellBlocked(cell))
                {
                    return false;
                }
                    
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

            if (placeableArea.HasTile(bottom) && placeableArea.GetTile(bottom) != voidTile 
                || placeableArea.HasTile(top) && placeableArea.GetTile(top) != voidTile)
            {
                return true;
            }
        }

        // left and right edge
        for (int y = 0; y < size.y; y++)
        {
            Vector3Int left = origin + new Vector3Int(-1, y, 0);
            Vector3Int right = origin + new Vector3Int(size.x, y, 0);

            if (placeableArea.HasTile(left) && placeableArea.GetTile(left) != voidTile 
                || placeableArea.HasTile(right) && placeableArea.GetTile(right) != voidTile)
            {
                return true;
            }
        }

        return false;
    }

    public bool ExpandRoom(Vector3Int origin, Vector2Int size, TileBase floorTile)
    {
        if (floorTile == null || !CanExpandRoom(origin, size))
            return false;

        var min = placeableArea.cellBounds.min;
        var max = placeableArea.cellBounds.max;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);
                placeableArea.SetTile(cell, floorTile);
            }
        }

        if(min.x > placeableArea.cellBounds.min.x || max.x < placeableArea.cellBounds.max.x)
        {
            FillVoid(new Vector3Int(origin.x, placeableArea.cellBounds.min.y, origin.z), new Vector2Int(size.x, max.y - min.y));
        }
        if(min.y > placeableArea.cellBounds.min.y || max.y < placeableArea.cellBounds.max.y)
        {
            FillVoid(new Vector3Int(placeableArea.cellBounds.min.x, origin.y, origin.z), new Vector2Int(max.x - min.x, size.y));
        }
        RefreshBottomEdges();
        decorationGridVisuals.RefreshGrid();

        return true;
    }

    public void FillVoid(Vector3Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);
                if (placeableArea.HasTile(cell))
                {
                    continue;
                }
                placeableArea.SetTile(cell, voidTile);
            }
        }
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
            if (!placeableArea.HasTile(cell) || placeableArea.GetTile(cell) == voidTile)
                continue;

            Vector3Int cellBelow = cell + Vector3Int.down;

            if (!placeableArea.HasTile(cellBelow) || placeableArea.GetTile(cellBelow) == voidTile)
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


    // for save
    public void GetFloorSaveData(BaseFloorSaveData baseFloor, AdditionalFloorSaveData additionalFloor, ItemDatabase database)
    {
        additionalFloor.tiles.Clear();

        TileBase floorTile = database.GetItem("floor_tile").floorTile;

        BoundsInt bounds = placeableArea.cellBounds;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        // additional floor bounds
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (placeableArea.GetTile(cell) != floorTile)
                continue;

            Vector3Int origin = SnapFloorOrigin(cell);

            // skip base floor
            if (origin.x >= baseFloor.x && origin.x + 3 <= baseFloor.x + baseFloor.width && origin.y >= baseFloor.y && origin.y + 3 <= baseFloor.y + baseFloor.height)
                continue;

            minX = Mathf.Min(minX, origin.x);
            minY = Mathf.Min(minY, origin.y);
            maxX = Mathf.Max(maxX, origin.x);
            maxY = Mathf.Max(maxY, origin.y);
        }

        if (minX == int.MaxValue)
        {
            additionalFloor.x = 0;
            additionalFloor.y = 0;
            additionalFloor.width = 0;
            additionalFloor.height = 0;
            return;
        }

        additionalFloor.x = minX;
        additionalFloor.y = minY;
        additionalFloor.width = (maxX - minX) / 3 + 1;
        additionalFloor.height = (maxY - minY) / 3 + 1;

        // save additional 3x3 tiles skipping base floor
        for (int y = 0; y < additionalFloor.height; y++)
        {
            for (int x = 0; x < additionalFloor.width; x++)
            {
                Vector3Int origin = new Vector3Int(minX + x * 3, minY + y * 3, 0);

                // skip base floor
                if (origin.x >= baseFloor.x && origin.x + 3 <= baseFloor.x + baseFloor.width && origin.y >= baseFloor.y && origin.y + 3 <= baseFloor.y + baseFloor.height)
                    continue;
                    
                if (placeableArea.GetTile(origin) == floorTile)
                {
                    int index = y * additionalFloor.width + x + 1;
                    additionalFloor.tiles.Add(index);
                }
            }
        }
    }

    public void LoadFloor(BaseFloorSaveData baseFloor, AdditionalFloorSaveData additionalFloor, ItemDatabase database)
    {
        placeableArea.ClearAllTiles();

        TileBase baseTile = database.GetItem(baseFloor.itemID).floorTile;
        TileBase floorTile = database.GetItem("floor_tile").floorTile;

        // load base floor
        for (int x = 0; x < baseFloor.width; x++)
        {
            for (int y = 0; y < baseFloor.height; y++)
            {
                placeableArea.SetTile(new Vector3Int(baseFloor.x + x, baseFloor.y + y, 0), baseTile);
            }
        }

        // load additional floor
        foreach (int index in additionalFloor.tiles)
        {
            int localX = (index - 1) % additionalFloor.width;
            int localY = (index - 1) / additionalFloor.width;

            int tileX = additionalFloor.x + localX * 3;
            int tileY = additionalFloor.y + localY * 3;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    placeableArea.SetTile(new Vector3Int(tileX + x, tileY + y, 0), floorTile);
                }
            }
        }

        RefreshBottomEdges();
        decorationGridVisuals.RefreshGrid();
    }


    public void ClearOccupiedCells()
    {
        occupiedCells.Clear();
    }
}