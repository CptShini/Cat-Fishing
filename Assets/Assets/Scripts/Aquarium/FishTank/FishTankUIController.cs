using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FishTankUIController : MonoBehaviour
{
    public static FishTankUIController Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text tankNameText;

    [Header("Fish list")]
    [SerializeField] private Transform listContent;
    [SerializeField] private FishTankFishRow rowPrefab;

    private FishTank currentTank;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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
        currentTank = tank;

        panel.SetActive(true);
        IsOpen = true;

        tankNameText.text = tank.gameObject.name;

        RefreshFishList();

        PauseController.SetPause(true);
    }

    private void RefreshFishList()
    {
        for (int i = listContent.childCount - 1; i >= 0; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        if (currentTank == null)
            return;

        foreach (Fish fish in currentTank.Fishes)
        {
            if (fish == null)
                continue;

            FishTankFishRow row = Instantiate(rowPrefab, listContent);

            row.Setup(fish, () => ReturnFishToInventory(fish));
        }
    }

    private void ReturnFishToInventory(Fish fish)
    {
        if (currentTank == null || fish == null)
            return;

        FishCollectionManager collection = FishCollectionManager.Instance;

        if (collection == null)
            return;

        // detach the fish from the tank first
        if (!currentTank.RemoveFish(fish))
            return;

        // add the fish back to the player's collection
        collection.AddFish(fish);

        RefreshFishList();
    }


    public void Close()
    {
        panel.SetActive(false);
        IsOpen = false;
        currentTank = null;

        PauseController.SetPause(false);
    }
}