using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaManager : MonoBehaviour, IDataPersistence
{

    public static EncyclopediaManager Instance { get; private set; }
        
    public bool IsEnabled { get; private set; }
        
    [SerializeField] private EncyclopediaSlot slotPrefab;   
    [SerializeField] private Transform grid;
    [SerializeField] private FishSpeciesDatabase species;

    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI catchHighscore;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI weightRange;
    [SerializeField] private TextMeshProUGUI description;
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
        bool pageSet = false;
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
                    if (!pageSet)
                    {
                        SetPageToFish(discovery);
                        pageSet = true;
                    }
                    break;
                }
            }
            if (!pageSet)
            {
                SetPageToUnknownFish(specie);
                pageSet = true;
            }
        }
    }

    public void SetPageToUnknownFish(FishSpecies species)
    {
        fishImage.sprite = species.Icon;
        fishImage.color = Color.black;
        title.text = "???";
        description.text = "";
        weightRange.text = "???-??? KG";
        catchHighscore.text = "???";
    }

    public void SetPageToFish(Fish fish)
    {
        fishImage.sprite = fish.Species.Icon;
        fishImage.color = Color.white;
        title.text = $"{fish.Species.Name}";
        description.text = fish.Species.Description;
        weightRange.text = $"{Math.Round(fish.Species.Weight.Min, 1)}-{Math.Round(fish.Species.Weight.Max, 1)} KG";

        catchHighscore.text = $"{Math.Round(fish.Weight, 1) } KG";
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
