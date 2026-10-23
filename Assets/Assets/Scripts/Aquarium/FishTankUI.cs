using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FishTankUI : MonoBehaviour
{
    public static FishTankUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text tankNameText;
    [SerializeField] private TMP_Text fishListText;

    public bool IsOpen { get; private set; }

    [SerializeField] private PlayerMovementAquarium playerMovementAquarium;

    private void Start()
    {
        Instance = this;
        panel.SetActive(false);
        IsOpen = false;
    }

    private void Update()
    {
        if (IsOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Open(FishTank tank)
    {
        panel.SetActive(true);
        IsOpen = true;

        if (playerMovementAquarium != null)
            playerMovementAquarium.enabled = false;

        tankNameText.text = tank.gameObject.name;

        fishListText.text = "";

        foreach (FishSO fish in tank.Fishes)
        {
            fishListText.text += fish.fishName + "\n";
        }

        if (fishListText.text == "")
        {
            fishListText.text = "No fish in this tank.";
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;

        if (playerMovementAquarium != null)
            playerMovementAquarium.enabled = true;
    }
}