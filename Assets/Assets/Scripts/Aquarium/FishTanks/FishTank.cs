using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTank : MonoBehaviour, IInteractable, IDayAware, IPlacedFurnitureInitializer
    {
        //[SerializeField] private string tankId; 
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

        public readonly UnityEvent TankChanged = new();
        
        public Tank Tank { get; private set; }



        public void InitializePlacedFurniture(PlacedFurniture furniture, GameData gameData)
        {
            if (furniture.itemSO.fishTankDefinition == null)
            {
                Debug.LogError($"FishTank '{name}' has no FishTankDefinition assigned.", this);
                return;
            }

            if (!System.Guid.TryParse(furniture.instanceID, out System.Guid tankId))
            {
                Debug.LogError($"Invalid tank ID: {furniture.instanceID}", this);
                return;
            }

            if (gameData != null && gameData.Tanks != null && gameData.Tanks.TryGetValue(furniture.instanceID, out TankDTO savedTank))
            {
                Tank = new Tank(savedTank, fishSpeciesDatabase);
            }
            else
            {
                var definition = furniture.itemSO.fishTankDefinition;
                Tank = new Tank(tankId, definition.tankName, definition.capacity);
            }
        }




        //private void Start() => Tank.TankChanged.AddListener(TankChanged.Invoke);
        //private void OnDestroy() => Tank.TankChanged.RemoveListener(TankChanged.Invoke);

        private void Start()
        {
            if (GetComponentInParent<PlacedFurniture>() == null)
                return;

            if (Tank == null)
            {
                Debug.LogError($"Placed FishTank on '{name}' was not initialized!", this);
                return;
            }

            Tank.TankChanged.AddListener(TankChanged.Invoke);
        }

        private void OnDestroy()
        {
            if (Tank != null)
                Tank.TankChanged.RemoveListener(TankChanged.Invoke);
        }
        
        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public void AddFish(TankFish fish) => Tank.AddFish(fish);
        public void RemoveFish(TankFish fish) => Tank.RemoveFish(fish);

        public void Feed() => Tank.Feed();

        public void DayPassed() => Tank.DayPassed();
    }
}
