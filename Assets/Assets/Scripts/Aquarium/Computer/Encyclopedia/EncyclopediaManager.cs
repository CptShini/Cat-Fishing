using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

public class EncyclopediaManager : MonoBehaviour, IDataPersistence
{

    public static EncyclopediaManager Instance { get; private set; }
        
    public bool IsEnabled { get; private set; }
        
    [SerializeField] private EncyclopediaSlot slotPrefab;   
    [SerializeField] private Transform grid;
    [SerializeField] private FishSpeciesDatabase species;

    private Dictionary<Guid, Fish> _discoveries;

    private void Awake()
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
        Init();
    }

    private void Init()
    {
        foreach (FishSpecies specie in species.GetSpecies())
        {
            EncyclopediaSlot slot = Instantiate(slotPrefab, grid, false);
            foreach (var discovery in _discoveries.Values)
            {
                if(discovery.Species == specie)
                {
                    slot.Setup(specie, discovery, true);
                    break;
                }
            }
            slot.Setup(specie);
        }
    }

    public void LoadData(GameData data)
    {
        _discoveries = data.Discoveries.ToDictionary(species);
    }

    public void SaveData(ref GameData data)
    {
        data.Discoveries = _discoveries.ToSerializableDictionary();
    }


}
