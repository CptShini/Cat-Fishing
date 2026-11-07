using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

public class FishCollectionManager : MonoBehaviour, IDataPersistence
{
    public static FishCollectionManager Instance { get; private set; }
    
    public readonly UnityEvent CollectionChanged = new();

    public IReadOnlyDictionary<Guid, Fish> CollectedFish => _collectedFish;
    private Dictionary<Guid, Fish> _collectedFish = new();
    
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

    public void Add(Fish fish)
    {
        _collectedFish.Add(fish.Id, fish);
        CollectionChanged.Invoke();
    }

    public bool Remove(Fish fish)
    {
        bool collectedFish = _collectedFish.ContainsKey(fish.Id);
        if (!collectedFish) return false;
        
        _collectedFish.Remove(fish.Id);
        CollectionChanged.Invoke();

        return true;
    }
    
    public void AddCollectibles(List<FishCollectible> fishCollectibles)
    {
        fishCollectibles.ForEach(fc => _collectedFish.Add(fc.fish.Id, fc.fish));
        CollectionChanged.Invoke();
    }

    public void LoadData(GameData data)
    {
        _collectedFish = data.Collection.ToDictionary(fishSpeciesDatabase);
        CollectionChanged.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        data.Collection = _collectedFish.ToSerializableDictionary();
    }
}