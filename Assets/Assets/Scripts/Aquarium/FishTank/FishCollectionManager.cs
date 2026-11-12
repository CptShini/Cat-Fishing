using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Aquarium.FishTank
{
    public class FishCollectionManager : MonoBehaviour, IDataPersistence
    {
        public static FishCollectionManager Instance { get; private set; }
    
        public readonly UnityEvent CollectionChanged = new();

        public IReadOnlyDictionary<Guid, TankFish> CollectedFish => _collectedFish;
        private Dictionary<Guid, TankFish> _collectedFish = new();

        private bool _shouldClear;
    
        [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Add(TankFish fish)
        {
            _collectedFish.Add(fish.Fish.Id, fish);
            CollectionChanged.Invoke();
        }

        public bool Remove(TankFish fish)
        {
            bool collectedFish = _collectedFish.ContainsKey(fish.Fish.Id);
            if (!collectedFish) return false;
        
            _collectedFish.Remove(fish.Fish.Id);
            CollectionChanged.Invoke();

            return true;
        }

        public void LoadData(GameData data)
        {
            _collectedFish = data.Collection.ToDictionary(dto => new TankFish(dto, fishSpeciesDatabase));
            CollectionChanged.Invoke();
        }

        public void SaveData(ref GameData data)
        {
            data.Collection = _collectedFish.ToSerializableDictionary(tf => new TankFishDTO(tf));
        }
    }
}