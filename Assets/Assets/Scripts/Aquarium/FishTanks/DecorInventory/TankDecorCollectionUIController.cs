using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.FishTanks.DecorInventory
{
     public class TankDecorCollectionUIController : MonoBehaviour
    {
        [SerializeField] private GameObject collectionPanel;
        [SerializeField] private Transform content;
        [SerializeField] private TankDecorInventorySlot slotPrefab;

        private TankDecorCollectionManager tankDecorCollection;

        private void Start()
        {
            TankDecorCollectionManager manager = TankDecorCollectionManager.Instance;

            tankDecorCollection = manager;
            tankDecorCollection.TankDecorCollectionChanged.AddListener(Refresh);
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

            if (tankDecorCollection.CollectedTankDecor.Count == 0)
            {
                collectionPanel.SetActive(false);
                return;
            }
            
            collectionPanel.SetActive(true);
            foreach (TankDecoration tankDecoration in tankDecorCollection.CollectedTankDecor.Values)
            {
                if (tankDecoration == null)
                    continue;

                TankDecorInventorySlot slot = Instantiate(slotPrefab, content);

                if (slot != null)
                    slot.SetTankDecor(tankDecoration);
            }
        }

        private void OnDestroy()
        {
            if (tankDecorCollection != null)
                tankDecorCollection.TankDecorCollectionChanged.RemoveListener(Refresh);
        }
    }
   
}