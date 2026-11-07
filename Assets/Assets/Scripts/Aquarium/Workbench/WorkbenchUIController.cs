using UnityEngine;

public class WorkbenchUIController : MonoBehaviour
{
    public static WorkbenchUIController Instance { get; private set; }

    public GameObject panel;
    public RoomCameraZoom roomCameraZoom;
    
    [SerializeField] private DecorationGridVisuals gridVisuals;
    [SerializeField] private PlacementController placementController;

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

    public void Open()
    {
        roomCameraZoom.EnterDecorationMode();
        gridVisuals.ShowGrid();
        panel.SetActive(true);
        IsOpen = true;

        PauseController.SetPause(true);
    }

    public void Close()
    {
        if (placementController != null && placementController.IsPlacingItem)
        {
            return;
        }

        gridVisuals.HideGrid();
        roomCameraZoom.ExitDecorationMode();
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
    }
}
