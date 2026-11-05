using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTank
{
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
            Fish fish = FishInventorySlot.DraggedFish;

            if (fish == null || fishTank == null)
                return;

            FishCollectionManager collection = FishCollectionManager.Instance;

            if (collection == null)
                return;

            // Check if the fish is in the collection before adding it to the tank
            if (!collection.CollectedFish.ContainsKey(fish.Id))
                return;

            // Attempt to add the fish to the tank and remove it from the collection
            if (!fishTank.AddFish(fish))
                return;
            if (!collection.Remove(fish))
            {
                fishTank.RemoveFish(fish);
                return;
            }
        }
    }
}