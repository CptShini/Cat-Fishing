using UnityEngine;


public class FurnitureSortingUtility  : MonoBehaviour
{
    public static string GetSortingLayer(FurnitureSortingLayer layer)
    {
        switch (layer)
        {
            case FurnitureSortingLayer.FloorDecor:
                return "FloorDecor";

            case FurnitureSortingLayer.World:
                return "World";

            case FurnitureSortingLayer.AboveWorld:
                return "AboveWorld";

            default:
                return "World";
        }
    }

    public static void ApplySorting(GameObject obj, ItemSO item)
    {
        string layerName = GetSortingLayer(item.sortingLayer);

        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.sortingLayerName = layerName;
            renderer.sortingOrder = item.sortingOrder;
        }
    }
}