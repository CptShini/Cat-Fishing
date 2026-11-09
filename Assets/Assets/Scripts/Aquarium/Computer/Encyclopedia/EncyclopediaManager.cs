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

    private Dictionary<Guid, Fish> _discoveries = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Init()
    {
        Debug.Log($"DISCOVERIES {_discoveries.Count}");
        foreach (FishSpecies specie in species.GetSpecies())
        {
            EncyclopediaSlot slot = Instantiate(slotPrefab, grid, false);

            slot.Setup(specie);

            foreach (var discovery in _discoveries.Values)
            {
                Debug.Log($"{discovery.Species.Id}, {specie.Id}");
                if(discovery.Species.Id == specie.Id)
                {
                    Debug.Log("AAAAHHHH");
                    slot.Setup(specie, discovery, true);
                    break;
                }
            }
        }
    }

    public void LoadData(GameData data)
    {
        _discoveries = data.Discoveries.ToDictionary(species);
        Init();
    }

    public void SaveData(ref GameData data)
    {
        data.Discoveries = _discoveries.ToSerializableDictionary();
    }


}
