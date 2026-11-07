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

    [Header("Fixed Furniture")]
    private RoomFixedFurniture selectedFixedFurniture;
    private Vector3 fixedFurnitureOriginalPosition;
    private Vector3Int fixedFurnitureOriginalOrigin;

    private Collider2D[] fixedFurnitureColliders;
    private bool[] fixedFurnitureColliderStates;

    [Header("Preview Colors")]
    [SerializeField] private Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    private ItemSO selectedItem;
    private GameObject ghostObject;

    private Vector3Int currentCell;
    private Vector2Int currentSize;

    private bool canPlace;

    public bool IsPlacingItem => selectedItem != null || selectedFixedFurniture != null;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // MOVING FIXED FURNITURE
        if (selectedFixedFurniture != null)
        {
            UpdateFixedFurniturePosition();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                TryPlaceFixedFurniture();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }

            return;
        }

        // NO ITEM SELECTED
        if (selectedItem == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                TryPickUpFurniture();
            }
            return;
        }

        // NORMAL ITEM PLACEMENT
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

        if (itemSO == null)
            return;

        selectedItem = itemSO;

        // FLOOR TILE
        if (itemSO.itemType == ItemType.FloorTile)
        {
            if (itemSO.floorTile == null)
            {
                Debug.LogWarning("Floor Tile is missing!");
                CancelPlacement();
                return;
            }

            currentSize = itemSO.floorSize;
            return;
        }

        // FURNITURE
        if (itemSO.prefab == null)
        {
            Debug.LogWarning("Furniture prefab is missing!");
            CancelPlacement();
            return;
        }

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

            if (furniture != null)
            {
                if (!inventory.AddItem(furniture.itemSO))
                    return;

                if (furniture.itemSO.blocksPlacement)
                {
                    roomGrid.UnregisterObject(furniture.origin, furniture.currentSize);
                }

                gridVisuals.UnregisterPlacedFurniture(furniture);

                Destroy(furniture.gameObject);
                return;
            }

            RoomFixedFurniture fixedFurniture = hit.GetComponentInParent<RoomFixedFurniture>();
            if (fixedFurniture != null)
            {
                StartMovingFixedFurniture(fixedFurniture);
                return;
            }
        }
    }

    private void UpdateFixedFurniturePosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float distanceToCamera = -mainCamera.transform.position.z;
        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, distanceToCamera);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        currentCell = roomGrid.WorldToCell(worldPosition);

        Vector3 placementPosition = roomGrid.GetPlacementPosition(currentCell, currentSize);

        selectedFixedFurniture.transform.position = placementPosition;

        canPlace = roomGrid.CanPlace(currentCell, currentSize, selectedFixedFurniture.blocksPlacement);

        gridVisuals.ShowFootprint(currentCell, currentSize, canPlace);
    }

    private void UpdateGhostPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float distanceToCamera = -mainCamera.transform.position.z;
        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, distanceToCamera);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        currentCell = roomGrid.WorldToCell(worldPosition);

        // FLOOR TILE
        if (selectedItem.itemType == ItemType.FloorTile)
        {
            currentCell = roomGrid.SnapFloorOrigin(currentCell);
            canPlace = roomGrid.CanExpandRoom(currentCell, currentSize);
            gridVisuals.ShowFootprint(currentCell, currentSize, canPlace);
            return;
        }

        // FURNITURE
        ghostObject.transform.position = roomGrid.GetPlacementPosition(currentCell, currentSize);
        canPlace = roomGrid.CanPlace(currentCell, currentSize, selectedItem.blocksPlacement);
        SetGhostColor(canPlace ? validColor : invalidColor);
        gridVisuals.ShowFootprint(currentCell, currentSize, canPlace);
    }

    private void TryPlaceItem()
    {
        if (!canPlace)
            return;

        // FLOOR TILE
        if (selectedItem.itemType == ItemType.FloorTile)
        {
            bool placed = roomGrid.ExpandRoom(currentCell, currentSize, selectedItem.floorTile);

            if (!placed)
                return;

            inventory.RemoveItem(selectedItem);

            // you can place floor tiles until you run out of them
            if (!inventory.HasItem(selectedItem))
            {
                CancelPlacement();
            }
            return;
        }

        // FURNITURE
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

        CancelPlacement();
    }

    private void TryPlaceFixedFurniture()
    {
        if (!canPlace)
            return;

        RoomFixedFurniture furniture = selectedFixedFurniture;

        if (furniture.blocksPlacement)
        {
            roomGrid.RegisterObject(currentCell, currentSize);
        }

        RestoreFixedFurnitureColliders();

        gridVisuals.RegisterFixedFurniture(furniture, currentCell);

        selectedFixedFurniture = null;

        gridVisuals.HideFootprint();
    }

    private void RestoreFixedFurnitureColliders()
    {
        if (fixedFurnitureColliders == null)
            return;

        for (int i = 0; i < fixedFurnitureColliders.Length; i++)
        {
            if (fixedFurnitureColliders[i] != null)
            {
                fixedFurnitureColliders[i].enabled = fixedFurnitureColliderStates[i];
            }
        }

        fixedFurnitureColliders = null;
        fixedFurnitureColliderStates = null;
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
        if (selectedFixedFurniture != null)
        {
            selectedFixedFurniture.transform.position = fixedFurnitureOriginalPosition;

            if (selectedFixedFurniture.blocksPlacement)
            {
                roomGrid.RegisterObject(fixedFurnitureOriginalOrigin, selectedFixedFurniture.size);
            }

            RestoreFixedFurnitureColliders();
            gridVisuals.RegisterFixedFurniture(selectedFixedFurniture, fixedFurnitureOriginalOrigin);
            selectedFixedFurniture = null;
        }

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

    // FIXED FURNITURE
    private void StartMovingFixedFurniture(RoomFixedFurniture furniture)
    {
        selectedFixedFurniture = furniture;

        fixedFurnitureOriginalPosition = furniture.transform.position;

        fixedFurnitureOriginalOrigin = furniture.GetOrigin(roomGrid.Grid);

        currentCell = fixedFurnitureOriginalOrigin;
        currentSize = furniture.size;

        // unregister the furniture from the grid
        if (furniture.blocksPlacement)
        {
            roomGrid.UnregisterObject(fixedFurnitureOriginalOrigin, currentSize);
        }

        // disable colliders
        fixedFurnitureColliders = furniture.GetComponentsInChildren<Collider2D>();
        fixedFurnitureColliderStates = new bool[fixedFurnitureColliders.Length];

        for (int i = 0; i < fixedFurnitureColliders.Length; i++)
        {
            fixedFurnitureColliderStates[i] = fixedFurnitureColliders[i].enabled;
            fixedFurnitureColliders[i].enabled = false;
        }

        // delete the furniture from the grid visuals
        gridVisuals.UnregisterFixedFurniture(furniture);
    }
}