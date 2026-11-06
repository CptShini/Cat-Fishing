using System;
using System.Collections.Generic;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using Range = Assets.Scripts.Helpers.Range;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishTankSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fishPrefab;
        [SerializeField] private Range spawnPosRangeX;
        [SerializeField] private Range spawnPosRangeY;
        
        private readonly Dictionary<Guid, GameObject> _fishes = new();

        public bool SpawnFish(Fish fish)
        {
            if (_fishes.ContainsKey(fish.Id))
                return false;
            
            float x = spawnPosRangeX.Random();
            float y = spawnPosRangeY.Random();
            Vector2 spawnPos = new(x, y);
            GameObject fishObject = Instantiate(fishPrefab, spawnPos, Quaternion.identity, transform);
            
            AquariumFish aquariumFish = fishObject.GetComponent<AquariumFish>();
            aquariumFish.Init(fish);
            
            _fishes.Add(fish.Id, fishObject);
            return true;
        }

        public bool RemoveFish(Fish fish)
        {
            if (!_fishes.Remove(fish.Id, out GameObject fishObject))
                return false;

            Destroy(fishObject);
            
            return true;
        }
    }
}