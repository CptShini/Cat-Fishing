using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryManager : MonoBehaviour, IDataPersistence
{
    public static DiscoveryManager Instance { get; private set; }
        
    public bool IsEnabled { get; private set; }

    private Dictionary<Guid, Fish> _discoveries;
    [SerializeField] private FishSpeciesDatabase species;

    public readonly UnityEvent<Fish> OnDiscovered = new();
    public readonly UnityEvent<Fish> OnNewRecord = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        Instance = this;
        _discoveries = new();
    }

    void Start()
    {
        CatchingMinigame.Instance.OnWin.AddListener(CheckForDiscovery);
    }
    private void CheckForDiscovery(Fish fish)
    {
        Debug.Log($"AAAAHHHHHH {fish.Strength}, {_discoveries.Count}");
        if(_discoveries.Count == 0)
        {
            NewDiscovery(fish);
            return;
        }
        foreach (Fish discoveredFish in _discoveries.Values)
        {
            Debug.Log(discoveredFish.RelativeWeight);
            if(fish.Species == discoveredFish.Species)
            {
                if(fish.Weight > discoveredFish.Weight)
                {
                    NewRecord(fish, discoveredFish);
                }
                return;
            }
        }
        NewDiscovery(fish);
    }

    private void NewRecord(Fish fish, Fish other)
    {
        _discoveries.Remove(other.Id);
        _discoveries.Add(fish.Id, fish);
        OnNewRecord.Invoke(fish);
    }

    private void NewDiscovery(Fish fish)
    {
        _discoveries.Add(fish.Id, fish);
        OnDiscovered.Invoke(fish);
    }

    public void LoadData(GameData data)
    {
        _discoveries = data.Discoveries.ToDictionary(species);
        //_discoveries.Clear();
    }

    public void SaveData(ref GameData data)
    {
        data.Discoveries = _discoveries.ToSerializableDictionary();
    }

}
