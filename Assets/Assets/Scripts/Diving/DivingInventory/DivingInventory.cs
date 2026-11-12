using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Audio;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Diving.Bars;
using UnityEngine;

namespace Assets.Scripts.Diving.DivingInventory
{
    public class DivingInventory : MonoBehaviour, IDataPersistence
    {
        public static DivingInventory Instance { get; private set; }
        
        public bool IsEnabled { get; private set; }

        private bool _savable;
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

        public bool Add(Collectible collectible)
        {
            if (!weightBar.TryAdd(collectible.Weight))
            {
                print($"No space in inventory for '{collectible.Name}' ({collectible.Weight:F1}kg)");
                AudioManager.Instance.Play(AudioId.InventoryFull);
                return false;
            }

            _collectibles.Add(collectible);
            CreateSlot(collectible);

            return true;
        }

        public bool CanAdd(Collectible collectible)
        {
            if (weightBar.CanAdd(collectible.Weight)) return true;
            
            print($"No space in inventory for '{collectible.Name}' ({collectible.Weight:F1}kg)");
            AudioManager.Instance.Play(AudioId.InventoryFull);
            return false;
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
        
        public void SetSavable() => _savable = true;

        public void LoadData(GameData data)
        {
            // No data to load
        }

        public void SaveData(ref GameData data)
        {
            if (!_savable) return;
            
            foreach (var fc in _collectibles.OfType<FishCollectible>())
            {
                TankFish tankFish = new(fc.fish);
                TankFishDTO tankFishDTO = new(tankFish);
                data.Collection.Add(tankFishDTO.Fish.Id, tankFishDTO);
            }
        }
    }
}
