using System;
using System.Collections.Generic;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Aquarium.FishTanks.Decor;
using Assets.Scripts.Aquarium.FishTanks.DecorInventory;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Fishies;
using UnityEngine;
using Range = Assets.Scripts.Helpers.Range;

namespace Assets.Scripts.Aquarium.FishTanks
{
    public class TankDecorationSpawner : MonoBehaviour
    {
        [SerializeField] private PlacedTankDecorationObject placedtankDecorPrefab;
        [SerializeField] private List<Transform> decorFrontLocations;
        [SerializeField] private Transform decorBackLocation; 
        [SerializeField] private Transform decorOutsideLocation; 

        private readonly Dictionary<Guid, PlacedTankDecorationObject> _decorations = new();

        public bool TryPlaceDecor(TankDecorInventorySlot tankDecorInventorySlot, Tank tank)
        {
            switch (tankDecorInventorySlot.TankDecor.Type.placementType)
            {
                
                case TankDecorationPlacementType.FRONT:
                    for (int i = 0; i < decorFrontLocations.Count; i++)
                    {
                        if(decorFrontLocations[i].childCount == 0)
                        {
                            return PlaceTankDecoration(tankDecorInventorySlot, decorFrontLocations[i], i, tank);
                        }
                    }
                    return false;
                case TankDecorationPlacementType.OUTSIDE:
                    return decorOutsideLocation.childCount == 0 
                           && PlaceTankDecoration(tankDecorInventorySlot, decorOutsideLocation, 0, tank);
                case TankDecorationPlacementType.BACK:
                    return decorBackLocation.childCount == 0
                           && PlaceTankDecoration(tankDecorInventorySlot, decorBackLocation, 0, tank);
                    ;
                default:
                    return false;
            }
        }
        
        public bool PlaceTankDecoration(TankDecorInventorySlot tankDecorInventorySlot, Transform spawnTransform, int index, Tank tank)
        {
            if (_decorations.ContainsKey(tankDecorInventorySlot.TankDecor.Id))
                return false;
            
            PlacedTankDecorationObject tankDecorObject = Instantiate(placedtankDecorPrefab, spawnTransform.position, Quaternion.identity);
            tankDecorObject.transform.SetParent(spawnTransform);
            
            PlacedTankDecoration placedTankDecoration = new PlacedTankDecoration(tankDecorInventorySlot.TankDecor, index);

            tankDecorObject.Init(placedTankDecoration);

            tank.AddDecoration(placedTankDecoration);
            
            _decorations.Add(tankDecorObject.PlacedTankDecoration.TankDecoration.Id, tankDecorObject);
            return true;
        }

        public bool PlaceTankDecoration(PlacedTankDecoration placedTankDecoration)
        {
            Transform spawnTransform;

            if (_decorations.ContainsKey(placedTankDecoration.TankDecoration.Id))
                return false;

           switch (placedTankDecoration.TankDecoration.Type.placementType)
            {
                case TankDecorationPlacementType.FRONT:
                    spawnTransform = decorFrontLocations[placedTankDecoration.PlacementIndex];
                    break;
                case TankDecorationPlacementType.OUTSIDE:
                    spawnTransform = decorOutsideLocation;
                    break;
                case TankDecorationPlacementType.BACK:
                    spawnTransform = decorBackLocation;
                    break;
                default:
                    return false;
            }
            
            PlacedTankDecorationObject tankDecorObject = Instantiate(placedtankDecorPrefab, spawnTransform.position, Quaternion.identity);
            tankDecorObject.transform.SetParent(spawnTransform);
            
            tankDecorObject.Init(placedTankDecoration);
            
            _decorations.Add(tankDecorObject.PlacedTankDecoration.TankDecoration.Id, tankDecorObject);
            return true;
        }

        public bool RemoveTankDecoration(PlacedTankDecoration placedTankDecoration)
        {
            if (!_decorations.Remove(placedTankDecoration.TankDecoration.Id, out PlacedTankDecorationObject tankDecorObject))
                return false;
            Destroy(tankDecorObject.gameObject);
            return true;
        }
        
        public void Clear()
        {
            foreach (PlacedTankDecorationObject tankDecorObject in _decorations.Values)
            {
                Destroy(tankDecorObject.gameObject);
            }
            
            _decorations.Clear();
        }
    }
}