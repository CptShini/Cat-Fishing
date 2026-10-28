using UnityEngine;

public class FishCollectionUIController : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private FishInventorySlot slotPrefab;

    private FishCollectionManager fishCollection;

    private void Start()
    {
        FishCollectionManager manager = FishCollectionManager.Instance;

        fishCollection = manager;
        fishCollection.OnCollectionChanged += Refresh;

        Refresh();
    }

    public void Refresh()
    {
        if (fishCollection == null)
            return;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (Fish fish in fishCollection.CollectedFish)
        {
            FishInventorySlot slot = Instantiate(slotPrefab, content);
            slot.SetFish(fish);
        }
    }
}
