using Assets.Scripts.Tutorial;
using UnityEngine;

public class WorkbenchUIController : MonoBehaviour
{
    public static WorkbenchUIController Instance { get; private set; }

    public GameObject panel;
    public RoomCameraZoom roomCameraZoom;
    
    [SerializeField] private DecorationGridVisuals gridVisuals;
    [SerializeField] private PlacementController placementController;

    [SerializeField] private GameObject inventoryPanel;

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

    public void HideInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public void Open()
    {
        roomCameraZoom.EnterDecorationMode();
        gridVisuals.ShowGrid();
        panel.SetActive(true);
        IsOpen = true;

        placementController.EnterDecorationMode();

        PauseController.IsGamePaused = true;

        TutorialUIController.Instance.ShowTutorial("Workbench");
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
