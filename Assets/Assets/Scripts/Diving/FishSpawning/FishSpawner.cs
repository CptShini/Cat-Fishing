using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Helpers;
using UnityEngine;
using RangeInt = Assets.Scripts.Helpers.RangeInt;

namespace Assets.Scripts.Diving.FishSpawning
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fishPrefab;

        [Space]
        
        [SerializeField] private RangeInt spawnCount;
        [SerializeField] private SpeciesSpawn[] spawnList;
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
            Transform randomSpawnPoint = spawnPoints.GetRandom();
            GameObject fish = Instantiate(fishPrefab, randomSpawnPoint.position, Quaternion.identity);
            FishController fc = fish.GetComponent<FishController>();
            
            FishSpecies randomSpecies = SelectRandomSpecies();
            fc.Init(randomSpecies);

            return;
            
            FishSpecies SelectRandomSpecies()
            {
                Dictionary<FishSpecies, int> speciesSpawns = spawnList
                    .ToDictionary(s => s.Species, s => s.Weight);
                return RandomHelper.WeightedRandom(speciesSpawns);
            }
        }
    }
}