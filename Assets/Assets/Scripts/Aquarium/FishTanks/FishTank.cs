using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTank : MonoBehaviour, IInteractable, IDataPersistence, IDayAware
    {
        [SerializeField] private string tankId; 
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

        public readonly UnityEvent TankChanged = new();
        
        public Tank Tank { get; private set; }

        private void Start() => Tank.TankChanged.AddListener(TankChanged.Invoke);
        
        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public void AddFish(TankFish fish) => Tank.AddFish(fish);
        public void RemoveFish(TankFish fish) => Tank.RemoveFish(fish);

        public void Feed() => Tank.Feed();
        
        public void LoadData(GameData data)
        {
            bool tankSaved = data.Tanks.TryGetValue(tankId, out TankDTO tankDTO);
            if (!tankSaved)
            {
                Debug.LogWarning($"Fish Tank \"{tankId}\" doesn't exist but was spawned in");
                Destroy(gameObject);
                return;
            }

            Tank = new(tankDTO, fishSpeciesDatabase);
        }

        public void SaveData(ref GameData data) => data.Tanks[tankId] = new(Tank);

        public void DayPassed() => Tank.DayPassed();
    }
}
