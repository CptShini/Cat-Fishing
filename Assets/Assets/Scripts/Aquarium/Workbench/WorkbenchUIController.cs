using UnityEngine;

public class WorkbenchUIController : MonoBehaviour
{
    public GameObject panel;
    public RoomCameraZoom roomCameraZoom;

    public bool IsOpen { get; private set; }
    
    void Start()
    {
        panel.SetActive(false);
        IsOpen = false;
    }

    public void Open()
    {
        roomCameraZoom.EnterDecorationMode();
        panel.SetActive(true);
        IsOpen = true;

        PauseController.SetPause(true);
    }

    public void Close()
    {
        roomCameraZoom.ExitDecorationMode();
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
    }
}
