using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private ItemSO[] items;

    public ItemSO GetItem(string itemID)
    {
        foreach (ItemSO item in items)
        {
            if (item != null && item.itemID == itemID)
                return item;
        }

        Debug.LogWarning("Item not found: " + itemID);
        return null;
    }

    public ItemSO GetItemByFloorTile(TileBase tile)
    {
        foreach (ItemSO item in items)
        {
            if (item != null && item.floorTile == tile)
                return item;
        }

        return null;
    }
}