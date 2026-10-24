using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FishTankUIController : MonoBehaviour
{
    public static FishTankUIController Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text tankNameText;
    [SerializeField] private TMP_Text fishListText;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        Instance = this;
        panel.SetActive(false);
        IsOpen = false;
        PauseController.SetPause(false);
    }

    private void Update()
    {
        if (IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Open(FishTank tank)
    {
        panel.SetActive(true);
        IsOpen = true;

        // Tank Name
        tankNameText.text = tank.gameObject.name;

        // Fish List (to change later to visible sprites of fishes instead of text)
        fishListText.text = "";

        foreach (FishSO fish in tank.Fishes)
        {
            fishListText.text += fish.fishName + "\n";
        }

        if (fishListText.text == "")
        {
            fishListText.text = "No fish in this tank.";
        }

        PauseController.SetPause(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;

        PauseController.SetPause(false);
    }
}