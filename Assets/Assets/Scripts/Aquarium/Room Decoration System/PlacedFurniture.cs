using System;
using UnityEngine;

public class PlacedFurniture : MonoBehaviour
{
    public ItemSO itemSO;
    public string instanceID;
    public Vector3Int origin;
    public Vector2Int currentSize;
    public bool isRegistered;

    public void Initialize(ItemSO item, Vector3Int cell, Vector2Int size, string savedInstanceID = null)
    {
        itemSO = item;
        origin = cell;
        currentSize = size;
        isRegistered = item.blocksPlacement;

        instanceID = string.IsNullOrEmpty(savedInstanceID) ? Guid.NewGuid().ToString() : savedInstanceID;
    }
}