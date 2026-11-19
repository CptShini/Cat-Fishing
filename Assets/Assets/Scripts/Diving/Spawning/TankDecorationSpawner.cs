using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Diving.TankDecor;
using Assets.Scripts.Helpers;
using UnityEngine;
using RangeInt = Assets.Scripts.Helpers.RangeInt;


namespace Assets.Scripts.Diving.Spawning
{
    public class TankDecorationSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject tankDecorPrefab;

        [Space]
        
        [SerializeField] private RangeInt spawnCount;
        [SerializeField] private TankDecorTypeSpawn[] spawnList;
        [SerializeField] private List<Transform> spawnPoints;

        private void Start()
        {
            int n = Mathf.Clamp(spawnCount.Random(), 0, spawnPoints.Count);
            for (int i = 0; i < n; i++)
            {
                SpawnRandomTankDecor();
            }
        }
        
        private void SpawnRandomTankDecor()
        {
            Transform randomSpawnPoint = spawnPoints.GetRandom();
            spawnPoints.Remove(randomSpawnPoint);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            GameObject TankDecor = Instantiate(tankDecorPrefab, randomSpawnPoint.position, rotation);
            


            TankDecorationType randomDecorType = SelectRandomSpecies();
            TankDecoration decoration = new TankDecoration(randomDecorType);
            TankDecor.GetComponent<TankDecorPickUp>().Init(decoration);

            return;
            
            TankDecorationType SelectRandomSpecies()
            {
                Dictionary<TankDecorationType, int> decorTypeSpawns = spawnList
                    .ToDictionary(s => s.DecorType, s => 1);
                return RandomHelper.WeightedRandom(decorTypeSpawns);
            }
        }
    }
}