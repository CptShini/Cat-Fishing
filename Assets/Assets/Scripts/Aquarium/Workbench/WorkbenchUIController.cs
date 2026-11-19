using UnityEngine;
using UnityEngine.InputSystem;

public class WorkbenchUIController : MonoBehaviour
{
    public static WorkbenchUIController Instance { get; private set; }

    public GameObject panel;
    public RoomCameraZoom roomCameraZoom;
    
    [SerializeField] private DecorationGridVisuals gridVisuals;
    [SerializeField] private PlacementController placementController;

    [SerializeField] private GameObject inventoryPanel;

    private bool isInventoryVisible = true;

    public bool IsOpen { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
        IsOpen = false;
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible;
        inventoryPanel.SetActive(isInventoryVisible);
    }

    public void Open()
    {
        roomCameraZoom.EnterDecorationMode();
        gridVisuals.ShowGrid();
        panel.SetActive(true);
        IsOpen = true;

        placementController.EnterDecorationMode();

        PauseController.IsGamePaused = true;
    }

    public void Close()
    {
        if (placementController != null && placementController.IsPlacingItem)
        {
            placementController.CancelPlacement();
        }

        placementController.ExitDecorationMode();

        gridVisuals.HideGrid();
        roomCameraZoom.ExitDecorationMode();
        panel.SetActive(false);
        IsOpen = false;

        PauseController.IsGamePaused = false;
    }
}
