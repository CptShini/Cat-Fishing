using UnityEngine;

public class WorkbenchUIController : MonoBehaviour
{
    public GameObject panel;
    public RoomCameraZoom roomCameraZoom;
    [SerializeField] private DecorationGridVisuals gridVisuals;

    public bool IsOpen { get; private set; }
    
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
        gridVisuals.HideGrid();
        roomCameraZoom.ExitDecorationMode();
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
    }
}
