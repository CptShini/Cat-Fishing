using Assets.Scripts.Audio.SFX;
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

        PauseController.IsGamePaused = true;
        SFXManager.Instance.Play(SFXId.ButtonClick);
    }

    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;

        PauseController.IsGamePaused = false;
        SFXManager.Instance.Play(SFXId.ButtonClick);
    }
}
