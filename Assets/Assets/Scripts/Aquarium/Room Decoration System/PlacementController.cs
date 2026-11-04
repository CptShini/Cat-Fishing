
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomGrid roomGrid;
    [SerializeField] private DecorInventoryManager inventory;
    [SerializeField] private Transform placedFurnitureRoot;
    [SerializeField] private Camera mainCamera;

    [Header("Preview Colors")]
    [SerializeField] private Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    private ItemSO selectedItem;
    private GameObject ghostObject;

    private Vector3Int currentCell;
    private Vector2Int currentSize;

    private bool canPlace;

    private void Update()
    {
        if (selectedItem == null || ghostObject == null)
            return;

        if (Mouse.current == null)
            return;

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
    }

    private void TryPlaceItem()
    {
        if (!canPlace)
            return;

        GameObject placedObject = Instantiate(selectedItem.prefab, ghostObject.transform.position, Quaternion.identity, placedFurnitureRoot);

        FurnitureVisual visual = placedObject.GetComponent<FurnitureVisual>();

        visual.SetSprite(selectedItem.itemIcon);

        PlacedFurniture furniture = placedObject.GetComponent<PlacedFurniture>();

        if (furniture == null)
        {
            furniture = placedObject.AddComponent<PlacedFurniture>();
        }

        furniture.Initialize(selectedItem, currentCell, currentSize);

        FurnitureSortingUtility.ApplySorting(placedObject, selectedItem);

        if (selectedItem.blocksPlacement)
        {
            roomGrid.RegisterObject(currentCell, currentSize);
        }

        inventory.RemoveItem(selectedItem);

        Debug.Log("Placed item: " + selectedItem.itemName);

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
    }
}