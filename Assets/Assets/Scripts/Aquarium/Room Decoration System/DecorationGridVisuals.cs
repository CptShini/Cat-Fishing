using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class DecorationGridVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap roomTilemap;
    [SerializeField] private Material lineMaterial;

    [Header("Grid Appearance")]
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private float gridLineWidth = 0.025f;

    [Header("Footprint Appearance")]
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private float footprintLineWidth = 0.055f;

    [Header("Rendering")]
    [SerializeField] private string sortingLayerName = "GridOverlay";
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private float zOffset = -0.05f;

    private readonly Dictionary<PlacedFurniture, LineRenderer> placedFrames = new();
    private readonly Dictionary<RoomFixedFurniture, LineRenderer> fixedFrames = new();
    private bool isGridVisible = false;
    [SerializeField] private Color placedColor = Color.yellow;

    private Transform gridRoot;
    private LineRenderer footprintRenderer;

    private void Awake()
    {
        CreateGrid();
        CreateFootprint();

        HideGrid();
    }

    private void CreateGrid()
    {
        GameObject root = new GameObject("GridVisualLines");
        root.transform.SetParent(transform, false);

        gridRoot = root.transform;

        BoundsInt bounds = roomTilemap.cellBounds;

        // vertical lines
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, bounds.yMin, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(x, bounds.yMax, 0));

            CreateLine("VerticalLine", start, end, gridColor, gridLineWidth, gridRoot);
        }

        // horizontal lines
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(bounds.xMin, y, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(bounds.xMax, y, 0));

            CreateLine("HorizontalLine", start, end, gridColor, gridLineWidth, gridRoot);
        }
    }

    private void CreateFootprint()
    {
        GameObject frame = new GameObject("FurnitureFootprint");
        frame.transform.SetParent(transform, false);

        footprintRenderer = frame.AddComponent<LineRenderer>();

        ConfigureLine(footprintRenderer, footprintLineWidth);

        footprintRenderer.positionCount = 5;
        footprintRenderer.loop = false;
        footprintRenderer.enabled = false;
    }

    private void CreateLine(string objectName, Vector3 start, Vector3 end, Color color, float width, Transform parent)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(parent, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        ConfigureLine(line, width);

        line.positionCount = 2;
        line.startColor = color;
        line.endColor = color;

        line.SetPosition(0, AddZOffset(start));
        line.SetPosition(1, AddZOffset(end));
    }

    private void ConfigureLine(LineRenderer line, float width)
    {
        line.useWorldSpace = true;
        line.sharedMaterial = lineMaterial;

        line.startWidth = width;
        line.endWidth = width;

        line.numCapVertices = 2;
        line.numCornerVertices = 2;

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;
    }

    private Vector3 AddZOffset(Vector3 position)
    {
        position.z += zOffset;
        return position;
    }

    public void ShowGrid()
    {
        isGridVisible = true;

        gridRoot.gameObject.SetActive(true);

        ShowPlacedFrames();
        ShowFixedFrames();
    }

    public void HideGrid()
    {
        isGridVisible = false;

        gridRoot.gameObject.SetActive(false);

        HideFootprint();
        HidePlacedFrames();
        HideFixedFrames();
    }

    public void ShowFootprint(Vector3Int origin, Vector2Int size, bool isValid)
    {
        if (footprintRenderer == null)
            return;

        Vector3 bottomLeft = grid.CellToWorld(origin);
        Vector3 topRight = grid.CellToWorld(origin + new Vector3Int(size.x, size.y, 0));
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);

        footprintRenderer.SetPosition(0, AddZOffset(bottomLeft));
        footprintRenderer.SetPosition(1, AddZOffset(bottomRight));
        footprintRenderer.SetPosition(2, AddZOffset(topRight));
        footprintRenderer.SetPosition(3, AddZOffset(topLeft));
        footprintRenderer.SetPosition(4, AddZOffset(bottomLeft));

        Color color = isValid ? validColor : invalidColor;

        footprintRenderer.startColor = color;
        footprintRenderer.endColor = color;

        footprintRenderer.enabled = true;
    }

    public void HideFootprint()
    {
        footprintRenderer.enabled = false;
    }


    
    public void RegisterPlacedFurniture(PlacedFurniture furniture)
    {
        if (furniture == null || placedFrames.ContainsKey(furniture))
            return;

        GameObject frame = new GameObject("PlacedFurnitureFrame");
        frame.transform.SetParent(transform, false);

        LineRenderer line = frame.AddComponent<LineRenderer>();

        ConfigureLine(line, footprintLineWidth);

        line.positionCount = 5;
        line.loop = false;

        line.startColor = placedColor;
        line.endColor = placedColor;

        placedFrames.Add(furniture, line);

        UpdatePlacedFurnitureFrame(furniture);
    }

    public void UnregisterPlacedFurniture(PlacedFurniture furniture)
    {
        if (furniture == null)
            return;

        if (placedFrames.TryGetValue(furniture, out LineRenderer line))
        {
            Destroy(line.gameObject);
            placedFrames.Remove(furniture);
        }
    }

    private void UpdatePlacedFurnitureFrame(PlacedFurniture furniture)
    {
        if (!placedFrames.TryGetValue(furniture, out LineRenderer line))
            return;

        Vector3 bottomLeft = grid.CellToWorld(furniture.origin);
        Vector3 topRight = grid.CellToWorld(furniture.origin + new Vector3Int(furniture.currentSize.x, furniture.currentSize.y, 0));
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);

        line.SetPosition(0, AddZOffset(bottomLeft));
        line.SetPosition(1, AddZOffset(bottomRight));
        line.SetPosition(2, AddZOffset(topRight));
        line.SetPosition(3, AddZOffset(topLeft));
        line.SetPosition(4, AddZOffset(bottomLeft));
    }

    public void ShowPlacedFrames()
    {
        foreach (var frame in placedFrames.Values)
            frame.enabled = true;
    }

    public void HidePlacedFrames()
    {
        foreach (var frame in placedFrames.Values)
            frame.enabled = false;
    }



    public void RegisterFixedFurniture(RoomFixedFurniture furniture, Vector3Int origin)
    {
        if (furniture == null || fixedFrames.ContainsKey(furniture))
            return;

        GameObject frame = new GameObject("FixedFurnitureFrame");
        frame.transform.SetParent(transform, false);

        LineRenderer line = frame.AddComponent<LineRenderer>();

        ConfigureLine(line, footprintLineWidth);

        line.positionCount = 5;
        line.loop = false;

        line.startColor = placedColor;
        line.endColor = placedColor;

        line.enabled = isGridVisible;

        fixedFrames.Add(furniture, line);

        UpdateFixedFurnitureFrame(furniture, origin);
    }

    private void UpdateFixedFurnitureFrame(RoomFixedFurniture furniture, Vector3Int origin)
    {
        if (!fixedFrames.TryGetValue(furniture, out LineRenderer line))
            return;

        Vector3 bottomLeft = grid.CellToWorld(origin);
        Vector3 topRight = grid.CellToWorld(origin + new Vector3Int(furniture.size.x, furniture.size.y, 0));
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);

        line.SetPosition(0, AddZOffset(bottomLeft));
        line.SetPosition(1, AddZOffset(bottomRight));
        line.SetPosition(2, AddZOffset(topRight));
        line.SetPosition(3, AddZOffset(topLeft));
        line.SetPosition(4, AddZOffset(bottomLeft));
    }

    private void ShowFixedFrames()
    {
        foreach (LineRenderer frame in fixedFrames.Values)
            frame.enabled = true;
    }

    private void HideFixedFrames()
    {
        foreach (LineRenderer frame in fixedFrames.Values)
            frame.enabled = false;
    }
}