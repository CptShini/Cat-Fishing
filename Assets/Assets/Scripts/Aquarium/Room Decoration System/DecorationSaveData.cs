using System;
using System.Collections.Generic;
using Assets.Scripts.Aquarium.Domain;

[Serializable]
public class DecorationSaveData
{
    public List<PlacedFurnitureSaveData> placedFurniture = new();
    public List<FixedFurnitureSaveData> fixedFurniture = new();
    public List<InventoryItemSaveData> inventory = new()
    {
        new()
        {
            itemID = "small_fish_tank",
            count = 1
        }
    };

    public BaseFloorSaveData baseFloor = new();
    public AdditionalFloorSaveData additionalFloor = new();

    public List<string> purchasedItemIDs = new();

}   
[Serializable]
public class BaseFloorSaveData
{
    public string itemID = "floor_tile";
    public int x = 3 * -2;
    public int y = 3 * 0;
    public int width = 3 * 4;
    public int height = 3 * 3;
}

[Serializable]
public class AdditionalFloorSaveData
{
    public int x;
    public int y;
    public int width;
    public int height;

    public List<int> tiles = new();
}

[Serializable]
public class PlacedFurnitureSaveData
{
    public string itemID;
    public string instanceID;

    public int x;
    public int y;
}

[Serializable]
public class FixedFurnitureSaveData
{
    public string furnitureID;

    public float x;
    public float y;
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemID;
    public int count;
    public string instanceID;
    public TankDTO tankData;
}