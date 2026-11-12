using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishTank : MonoBehaviour, IInteractable, IDataPersistence
    {
        [SerializeField] private string tankId; 
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

        private Tank _tank;
        
        public Guid Id => _tank.Id;
        public string Name => _tank.Name;
        public IReadOnlyList<TankFish> Fish => _tank.Fishes;

        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public void AddFish(TankFish fish) => _tank.AddFish(fish);
        public void RemoveFish(TankFish fish) => _tank.RemoveFish(fish);

        public void LoadData(GameData data)
        {
            bool tankSaved = data.Tanks.TryGetValue(tankId, out TankDTO tankDTO);
            if (!tankSaved)
            {
                Debug.LogWarning($"Fish Tank \"{tankId}\" doesn't exist but was spawned in");
                Destroy(gameObject);
                return;
            }

            _tank = new(tankDTO, fishSpeciesDatabase);
        }

        public void SaveData(ref GameData data)
        {
            data.Tanks[tankId] = new(_tank);
        }
    }
}
