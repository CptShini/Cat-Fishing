using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomGrid roomGrid;
    [SerializeField] private DecorInventoryManager inventory;
    [SerializeField] private Transform placedFurnitureRoot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private DecorationGridVisuals gridVisuals;

    [Header("Preview Colors")]
    [SerializeField] private Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    private ItemSO selectedItem;
    private GameObject ghostObject;

    private Vector3Int currentCell;
    private Vector2Int currentSize;

    private bool canPlace;

    public bool IsPlacingItem => selectedItem != null;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (selectedItem == null || ghostObject == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                TryPickUpFurniture();
            }
            return;
        }

        UpdateGhostPosition();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TryPlaceItem();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    public void SelectItem(ItemSO itemSO)
    {
        CancelPlacement();

        if (itemSO == null || itemSO.prefab == null)
        {
            Debug.LogWarning("Item or prefab is null!");
            return;
        }

        selectedItem = itemSO;
        currentSize = itemSO.size;

        ghostObject = Instantiate(itemSO.prefab);
        FurnitureVisual visual = ghostObject.GetComponent<FurnitureVisual>();
        visual.SetSprite(itemSO.itemIcon);

        foreach (Collider2D col in ghostObject.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        FurnitureSortingUtility.ApplySorting(ghostObject, selectedItem);

        SetGhostColor(validColor);

        Debug.Log("Selected item: " + itemSO.itemName);

        AudioManager.Instance.PlayPickUpItem();
    }

    private void TryPickUpFurniture()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float distanceToCamera = -mainCamera.transform.position.z;

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, distanceToCamera);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            PlacedFurniture furniture = hit.GetComponentInParent<PlacedFurniture>();

            if (furniture == null)
                continue;

            if (!inventory.AddItem(furniture.itemSO))
                return;

            if (furniture.itemSO.blocksPlacement)
            {
                roomGrid.UnregisterObject(furniture.origin, furniture.currentSize);
            }

            gridVisuals.UnregisterPlacedFurniture(furniture);

            Debug.Log("Picked up furniture: " + furniture.itemSO.itemName);

            Destroy(furniture.gameObject);

            return;
        }
    }

    private void UpdateGhostPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float distanceToCamera = -mainCamera.transform.position.z;

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, distanceToCamera);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        currentCell = roomGrid.WorldToCell(worldPosition);

        ghostObject.transform.position = roomGrid.GetPlacementPosition(currentCell, currentSize);

        canPlace = roomGrid.CanPlace(currentCell, currentSize, selectedItem.blocksPlacement);

        SetGhostColor(canPlace ? validColor : invalidColor);
        
        gridVisuals.ShowFootprint(currentCell, currentSize, canPlace);
    }

    private void TryPlaceItem()
    {
        if (!canPlace) {
            AudioManager.Instance.PlayPlacementWrong();
            return;
        }

        GameObject placedObject = Instantiate(selectedItem.prefab, ghostObject.transform.position, Quaternion.identity, placedFurnitureRoot);

        FurnitureVisual visual = placedObject.GetComponent<FurnitureVisual>();
        visual.SetSprite(selectedItem.itemIcon);

        SetupFurnitureCollider(placedObject, selectedItem);

        PlacedFurniture furniture = placedObject.GetComponent<PlacedFurniture>();

        if (furniture == null)
        {
            furniture = placedObject.AddComponent<PlacedFurniture>();
        }

        furniture.Initialize(selectedItem, currentCell, currentSize);
        gridVisuals.RegisterPlacedFurniture(furniture);

        FurnitureSortingUtility.ApplySorting(placedObject, selectedItem);

        if (selectedItem.blocksPlacement)
        {
            roomGrid.RegisterObject(currentCell, currentSize);
        }

        inventory.RemoveItem(selectedItem);

        Debug.Log("Placed item: " + selectedItem.itemName);

        AudioManager.Instance.PlayPlacementSuccess();

        CancelPlacement();
    }

    private void SetGhostColor(Color color)
    {
        if (ghostObject == null)
            return;

        foreach (SpriteRenderer sr in ghostObject.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

    public void CancelPlacement()
    {
        if (ghostObject != null)
        {
            Destroy(ghostObject);
        }

        ghostObject = null;
        selectedItem = null;
        gridVisuals.HideFootprint();
    }

    private void SetupFurnitureCollider(GameObject furniture, ItemSO item)
    {
        SpriteRenderer spriteRenderer = furniture.GetComponentInChildren<SpriteRenderer>();

        PolygonCollider2D collider = spriteRenderer.gameObject.GetComponent<PolygonCollider2D>();

        if (collider == null)
            collider = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();

        collider.isTrigger = item.sortingLayer != FurnitureSortingLayer.World;

        Sprite sprite = spriteRenderer.sprite;

        int shapeCount = sprite.GetPhysicsShapeCount();

        collider.pathCount = shapeCount;

        for (int i = 0; i < shapeCount; i++)
        {
            List<Vector2> points = new List<Vector2>();
            sprite.GetPhysicsShape(i, points);
            collider.SetPath(i, points);
        }
    }
}