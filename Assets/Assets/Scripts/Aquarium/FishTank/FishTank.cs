using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishTank : MonoBehaviour, IInteractable, IDataPersistence
    {
        [SerializeField] private string tankId;
        [SerializeField] private string tankName;
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
        
        private Dictionary<Guid, Fish> _fish = new();
        
        public string Id => tankId;
        public string Name => tankName;
        public IReadOnlyCollection<Fish> Fish => _fish.Values;

        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public bool AddFish(Fish fish) => _fish.TryAdd(fish.Id, fish);

        public bool RemoveFish(Fish fish)
        {
            bool containsFish = _fish.ContainsKey(fish.Id);
            if (!containsFish) return false;

            _fish.Remove(fish.Id);

            return true;
        }

        public void LoadData(GameData data)
        {
            bool tankSaved = data.Tanks.ContainsKey(tankId);
            if (!tankSaved) return;
            
            TankData tankData = data.Tanks[tankId];
            _fish = tankData.Fishes.ToDictionary(fishSpeciesDatabase);
        }

        public void SaveData(ref GameData data)
        {
            List<FishDTO> fishDTOs = _fish.Values
                .ToDTOs()
                .ToList();
            
            if (data.Tanks.TryGetValue(tankId, out TankData tank))
            {
                tank.Fishes = fishDTOs;
            }
            else
            {
                tank = new(tankId, fishDTOs);
                data.Tanks.Add(tankId, tank);
            }
        }
    }
}
