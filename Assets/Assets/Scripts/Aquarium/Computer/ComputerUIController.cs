using UnityEngine;

public class ComputerUIController : MonoBehaviour
{
    public GameObject panel;

    public bool IsOpen { get; private set; }
    
    void Start()
    {
        panel.SetActive(false);
        IsOpen = false;
    }

    public void Open()
    {
        panel.SetActive(true);
        IsOpen = true;

        PauseController.SetPause(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
    }
}
