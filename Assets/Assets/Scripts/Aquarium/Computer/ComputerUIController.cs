using UnityEngine;

public class ComputerUIController : MonoBehaviour
{
    public static ComputerUIController Instance { get; private set; }

    public GameObject panel;

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
        panel.SetActive(true);
        IsOpen = true;

        PauseController.SetPause(true);
        AudioManager.Instance.PlayButtonClick();
    }

    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
        AudioManager.Instance.PlayButtonClick();
    }
}
