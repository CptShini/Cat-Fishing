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
        [SerializeField] private TankDecorPickUp tankDecorPrefab;

        [Space]
        
        [SerializeField] private RangeInt spawnCount;
        [SerializeField] private WeightedSpawnList<TankDecorationType> spawnList;
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
            bool noSpawn = !spawnList.GetRandomSpawn(out TankDecorationType randomDecorType);
            if (noSpawn) return;
            
            Transform randomSpawnPoint = spawnPoints.GetRandom();
            spawnPoints.Remove(randomSpawnPoint);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            TankDecorPickUp TankDecor = Instantiate(tankDecorPrefab, randomSpawnPoint.position, rotation);
            TankDecoration decoration = new(randomDecorType);
            TankDecor.Init(decoration);
        }
    }
}