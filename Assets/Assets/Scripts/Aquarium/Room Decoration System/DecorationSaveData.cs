using System;
using System.Collections.Generic;

[Serializable]
public class DecorationSaveData
{
    public List<PlacedFurnitureSaveData> placedFurniture = new();
    public List<FixedFurnitureSaveData> fixedFurniture = new();
    public List<InventoryItemSaveData> inventory = new();
    public List<PlacedFloorSaveData> placedFloor = new();

    public List<string> purchasedItemIDs = new();
}

[Serializable]
public class PlacedFurnitureSaveData
{
    public string itemID;

    public int x;
    public int y;
    public int z;
}

[Serializable]
public class FixedFurnitureSaveData
{
    public string furnitureID;

    public float x;
    public float y;
    public float z;
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemID;
    public int count;
}

[Serializable]
public class PlacedFloorSaveData
{
    public string itemID;

    public int x;
    public int y;
    public int z;
}