using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Furniture;

    [Header("Placement")]
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;

    public bool blocksPlacement = true;

    [Header("Rendering")]
    public FurnitureSortingLayer sortingLayer = FurnitureSortingLayer.World;
    public int sortingOrder = 0;

    [Header("Floor Placement")]
    public TileBase floorTile;
    public Vector2Int floorSize = new Vector2Int(3, 3);
}

public enum ItemType
{
    Furniture,
    FloorTile
}

public enum FurnitureSortingLayer
{
    FloorDecor,
    World,
    AboveWorld
}