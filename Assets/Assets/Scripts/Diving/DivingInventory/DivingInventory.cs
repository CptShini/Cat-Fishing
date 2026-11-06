using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Diving.Bars;
using UnityEngine;

namespace Assets.Scripts.Diving.DivingInventory
{
    public class DivingInventory : MonoBehaviour
    {
        public static DivingInventory Instance { get; private set; }
        
        public bool IsEnabled { get; private set; }
        
        private List<Collectible> _collectibles;
        
        [SerializeField] private DivingInventorySlot slotPrefab;
        [SerializeField] public GameObject panel;
        [SerializeField] private Transform grid;
        [SerializeField] private WeightBar weightBar;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _collectibles = new();
            weightBar.SetMaxValue(DivingPlayerStats.Instance.carryingCapacity, 0f);
        }
        
        public List<T> GetCollectiblesOfType<T>() where T : Collectible => 
            _collectibles.OfType<T>().ToList();

        public bool Add(Collectible collectible)
        {
            if (!weightBar.TryAdd(collectible.Weight))
            {
                print($"No space in inventory for '{collectible.Name}' ({collectible.Weight:F1}kg)");
                return false;
            }

            _collectibles.Add(collectible);
            CreateSlot(collectible);

            return true;
        }

        public bool CanAdd(Collectible collectible){
            if (!weightBar.CanAdd(collectible.Weight))
            {
                print($"No space in inventory for '{collectible.Name}' ({collectible.Weight:F1}kg)");
                return false;
            }
            return true;
        }
        
        public void Remove(Collectible collectible)
        {
            _collectibles.Remove(collectible);
            weightBar.CurrentValue -= collectible.Weight;
        }

        private void CreateSlot(Collectible collectible)
        {
            DivingInventorySlot slot = Instantiate(slotPrefab, grid, false);
            slot.Setup(collectible);
        }

        public void Drag(DivingInventorySlot inventorySlot)
        {
            Debug.Log("DRAGGING");
        }

        public void Toggle() => SetEnabled(!IsEnabled);
        
        public void SetEnabled(bool enable)
        {
            IsEnabled = enable;
            panel.SetActive(IsEnabled);
        }
    }
}
