using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTank : MonoBehaviour, IInteractable, IDayAware, IPlacedFurnitureInitializer
    {
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
        
        public Tank Tank { get; private set; }
        public FishTankDefinitionSO Definition { get; private set; }

        public void InitializePlacedFurniture(PlacedFurniture furniture, GameData gameData)
        {
            if (!furniture.itemSO.fishTankDefinition)
            {
                Debug.LogError($"FishTank '{name}' has no FishTankDefinition assigned.", this);
                return;
            }

            Definition = furniture.itemSO.fishTankDefinition;

            if (!System.Guid.TryParse(furniture.instanceID, out System.Guid tankId))
            {
                Debug.LogError($"Invalid tank ID: {furniture.instanceID}", this);
                return;
            }

            if (gameData is { Tanks: not null } && gameData.Tanks.TryGetValue(furniture.instanceID, out TankDTO savedTank))
            {
                Tank = new(savedTank, fishSpeciesDatabase);
            }
            else
            {
                Tank = new(tankId, Definition.tankName, Definition.GetCapacity(1));
            }
        }

        public void RestoreTank(TankDTO tankDTO)
        {
            Tank = new Tank(tankDTO, fishSpeciesDatabase);
        }
        
        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public void DayStarted() => Tank.DayStarted();
    }
}
