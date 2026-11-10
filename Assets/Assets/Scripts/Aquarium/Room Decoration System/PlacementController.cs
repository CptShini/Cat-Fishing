using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Assets.Scripts.Audio;
using UnityEngine.Events;
using NavMeshPlus.Components;

public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

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

    [Header("Audio Timer CD")]
    [SerializeField] private float wrongPlacementCooldown = 0.5f;
    private float nextWrongPlacementSoundTime;

    private ItemSO selectedItem;
    private GameObject ghostObject;

    private Vector3Int currentCell;
    private Vector2Int currentSize;

    private bool canPlace;

    public readonly UnityEvent OnPlaced = new();
    public readonly UnityEvent OnPickedUp = new();
    public bool IsPlacingItem => selectedItem != null || selectedFixedFurniture != null;

    private bool isDecorationMode = false;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
    }

    public void EnterDecorationMode()
    {
        isDecorationMode = true;
    }

    public void ExitDecorationMode()
    {
        isDecorationMode = false;
        CancelPlacement();
    }

    private void Update()
    {
        if (!isDecorationMode)
            return;

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
        AudioManager.Instance.Play(AudioId.PickUpItem);
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
                PickUpPlacedFurniture(furniture);
                return;
            }

            RoomFixedFurniture fixedFurniture = hit.GetComponentInParent<RoomFixedFurniture>();
            if (fixedFurniture != null)
            {
                StartMovingFixedFurniture(fixedFurniture);
                return;
            }
        }
 
        // picking up by checking bounds and not only sprite
        PlacedFurniture[] allFurniture = FindObjectsByType<PlacedFurniture>(FindObjectsSortMode.None);

        foreach (PlacedFurniture furniture in allFurniture)
        {
            Vector3Int origin = furniture.origin;

            Bounds bounds = new Bounds(roomGrid.GetPlacementPosition(origin, furniture.currentSize),
                new Vector3(furniture.currentSize.x * roomGrid.Grid.cellSize.x, furniture.currentSize.y * roomGrid.Grid.cellSize.y, 0.1f));

            if (bounds.Contains(worldPosition))
            {
                PickUpPlacedFurniture(furniture);
                return;
            }
        }
    }

    private void PickUpPlacedFurniture(PlacedFurniture furniture)
    {
        if (!inventory.AddItem(furniture.itemSO))
            return;

        if (furniture.itemSO.blocksPlacement)
        {
            roomGrid.UnregisterObject(furniture.origin, furniture.currentSize);
        }

        gridVisuals.UnregisterPlacedFurniture(furniture);

        Destroy(furniture.gameObject);

        OnPickedUp.Invoke();
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
        {
            if (Time.time >= nextWrongPlacementSoundTime)
            {
                AudioManager.Instance.Play(AudioId.PlacementWrong);
                nextWrongPlacementSoundTime = Time.time + wrongPlacementCooldown;
            }

            return;
        }

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
        FurnitureSortingUtility.ApplySorting(placedObject, selectedItem);

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
        OnPlaced.Invoke();
        AudioManager.Instance.Play(AudioId.PlacementSuccess);

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

        if (!collider.isTrigger)
        {
            NavMeshModifier navMod = spriteRenderer.gameObject.AddComponent<NavMeshModifier>();

            navMod.overrideArea = true;
            navMod.area = 1;
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