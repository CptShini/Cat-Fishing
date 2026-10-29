using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class FishTankUIIcon : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image tankImage;

    [Header("Tank object on the map")]
    [SerializeField] private FishTank fishTank;

    public FishTank Tank => fishTank;

    private void Awake()
    {
        if (tankImage == null)
            tankImage = GetComponent<Image>();

        tankImage.raycastTarget = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        FishSO fish = FishInventorySlot.DraggedFish;

        if (fish == null || fishTank == null)
            return;

        FishCollectionManager collection = FishCollectionManager.Instance;

        if (collection == null)
            return;

        // Check if the fish is in the collection before adding it to the tank
        if (!collection.CollectedFish.Contains(fish))
            return;

        // Attempt to add the fish to the tank and remove it from the collection
        if (!fishTank.AddFish(fish))
            return;
        if (!collection.RemoveFishItem(fish))
        {
            fishTank.RemoveFish(fish);
            return;
        }
    }
}