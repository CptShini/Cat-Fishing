using Assets.Scripts.Aquarium.FishTanks.FishInventory;
using Assets.Scripts.Data.Domain.Aquarium;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTankUIDropZone : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            TankFish fish = FishInventorySlot.DraggedFish;
            if (fish == null) return;
            
            FishTankUIController.Instance.AddFish(fish);
            FishCollectionManager.Instance.Remove(fish);
        }
    }
}