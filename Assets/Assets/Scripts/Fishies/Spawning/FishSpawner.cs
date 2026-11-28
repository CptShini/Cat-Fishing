using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Helpers;
using UnityEngine;
using RangeInt = Assets.Scripts.Helpers.RangeInt;

namespace Assets.Scripts.Fishies.Spawning
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private FishController fishPrefab;

        [Space]
        
        [SerializeField] private RangeInt spawnCount;
        [SerializeField] private WeightedSpawnList<FishSpecies> spawnList;
        [SerializeField] private List<Transform> spawnPoints;

        private void Start()
        {
            int n = spawnCount.Random();
            for (int i = 0; i < n; i++)
            {
                SpawnRandomFish();
            }
        }
        
        private void SpawnRandomFish()
        {
            bool noSpawn = !spawnList.GetRandomSpawn(out FishSpecies randomSpecies);
            if (noSpawn) return;
            
            Transform randomSpawnPoint = spawnPoints.GetRandom();
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            FishController fish = Instantiate(fishPrefab, randomSpawnPoint.position, randomRotation);
            fish.Init(randomSpecies);
        }
    }
}