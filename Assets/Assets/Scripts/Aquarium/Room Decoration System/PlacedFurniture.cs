using UnityEngine;

public class PlacedFurniture : MonoBehaviour
{
    public ItemSO itemSO;
    public Vector3Int origin;
    public Vector2Int currentSize;
    public bool isRegistered;

    public void Initialize(ItemSO item, Vector3Int cell, Vector2Int size)
    {
        itemSO = item;
        origin = cell;
        currentSize = size;
        isRegistered = item.blocksPlacement;
    }
}