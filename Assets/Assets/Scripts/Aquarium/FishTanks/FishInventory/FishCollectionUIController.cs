using Assets.Scripts.Data.Domain.Aquarium;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTanks.FishInventory
{
    public class FishCollectionUIController : MonoBehaviour
    {
        [SerializeField] private GameObject collectionPanel;
        [SerializeField] private Transform content;
        [SerializeField] private FishInventorySlot slotPrefab;

        private FishCollectionManager fishCollection;

        private void Start()
        {
            FishCollectionManager manager = FishCollectionManager.Instance;

            fishCollection = manager;
            fishCollection.CollectionChanged.AddListener(Refresh);

            Refresh();
        }
    
        public void Refresh()
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Transform child = content.GetChild(i);

                if (child != null)
                    Destroy(child.gameObject);
            }

            if (fishCollection.CollectedFish.Count == 0)
            {
                collectionPanel.SetActive(false);
                return;
            }
            
            collectionPanel.SetActive(true);
            foreach (TankFish fish in fishCollection.CollectedFish.Values)
            {
                if (fish == null)
                    continue;

                FishInventorySlot slot = Instantiate(slotPrefab, content);

                if (slot != null)
                    slot.SetFish(fish);
            }
        }

        private void OnDestroy()
        {
            if (fishCollection != null)
                fishCollection.CollectionChanged.RemoveListener(Refresh);
        }
    }
}
