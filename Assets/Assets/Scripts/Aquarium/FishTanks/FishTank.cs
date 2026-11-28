using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class FishTank : MonoBehaviour, IInteractable, IDayAware, IPlacedFurnitureInitializer
    {
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;
        [SerializeField] private TankDecorationTypeDatabase tankDecorationTypeDatabase;
        
        public Tank Tank { get; private set; }
        public FishTankDefinitionSO Definition { get; private set; }

        [SerializeField] private GameObject exclamationImage;
        [SerializeField, Min(0f)] private float dirtinessWarningThreshold = 3f;


        void Start()
        {
            exclamationImage.GetComponent<SpriteRenderer>().sortingLayerName = FurnitureSortingUtility.GetSortingLayer(FurnitureSortingLayer.AboveWorld);
        }

        public void InitializePlacedFurniture(PlacedFurniture furniture, GameData gameData)
        {
            if (!furniture.itemSO.fishTankDefinition)
            {
                Debug.LogError($"FishTank '{name}' has no FishTankDefinition assigned.", this);
                return;
            }

            Definition = furniture.itemSO.fishTankDefinition;

            System.Guid tankId = furniture.instanceID.TryParseGuid();
            if (gameData != null && gameData.Tanks.TryGetValue(furniture.instanceID, out TankDTO savedTank))
            {
                Tank = new(savedTank, fishSpeciesDatabase, tankDecorationTypeDatabase);
            }
            else
            {
                Tank = new(tankId, Definition.tankName, Definition.GetCapacity(1), Definition.decorCapacity);
            }

            UpdateDirtinessWarning();
        }

        public void RestoreTank(TankDTO tankDTO)
        {
            Tank = new(tankDTO, fishSpeciesDatabase, tankDecorationTypeDatabase);
            UpdateDirtinessWarning();
        }
        
        public bool CanInteract() => true;

        public void Interact() => FishTankUIController.Instance.Open(this);

        public void DayStarted()
        {
            Tank.DayStarted();
            UpdateDirtinessWarning();
        }

        public void UpdateDirtinessWarning() => 
            exclamationImage.SetActive(Tank.Dirtiness >= dirtinessWarningThreshold);
    }
}
