using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.Aquarium.FishTanks;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Save")]
    public string itemID;

    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;
    public float EntertainmentValue;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Furniture;

    [Header("Placement")]
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;

    public bool blocksPlacement = true;

    [Header("Rendering")]
    public FurnitureSortingLayer sortingLayer = FurnitureSortingLayer.World;
    public int sortingOrder = 0;

    [Header("Split Rendering")]
    public bool splitRendering = false;
    [Range(0f, 1f)] public float splitHeight = 0.5f;

    [Header("Floor Placement")]
    public TileBase floorTile;
    public Vector2Int floorSize = new Vector2Int(3, 3);

    [Header("Fish Tank")]
    public FishTankDefinitionSO fishTankDefinition;
}

public enum ItemType
{
    Furniture,
    FloorTile,
    FishTank
}

public enum FurnitureSortingLayer
{
    FloorDecor,
    World,
    AboveWorld
}