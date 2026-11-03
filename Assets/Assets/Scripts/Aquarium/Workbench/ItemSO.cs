using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;

    [Header("Placement")]
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;

    public bool blocksPlacement = true;

    [Header("Rendering")]
    public FurnitureSortingLayer sortingLayer = FurnitureSortingLayer.World;
    public int sortingOrder = 0;
}

public enum FurnitureSortingLayer
{
    FloorDecor,
    World,
    AboveWorld
}