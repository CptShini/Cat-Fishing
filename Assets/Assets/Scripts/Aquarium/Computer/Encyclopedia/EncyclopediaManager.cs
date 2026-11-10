using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.Computer.Encyclopedia
{
    public class EncyclopediaManager : MonoBehaviour, IDataPersistence
    {
        public static EncyclopediaManager Instance { get; private set; }
        
        public bool IsEnabled { get; private set; }
        
        [SerializeField] private EncyclopediaSlot slotPrefab;   
        [SerializeField] private Transform grid;
        [SerializeField] private FishSpeciesDatabase speciesDb;

        [SerializeField] private Image fishImage;
        [SerializeField] private TextMeshProUGUI catchHighscore;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI weightRange;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI collection;
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
            bool pageSet = false;
            foreach (FishSpecies species in speciesDb.Species)
            {
                EncyclopediaSlot slot = Instantiate(slotPrefab, grid, false);
                slot.Setup(species);

                foreach (Fish discovery in _discoveries.Values.Where(discovery => discovery.Species.Id == species.Id))
                {
                    slot.Setup(species, discovery, true);
                    if (!pageSet)
                    {
                        SetPageToFish(discovery);
                        pageSet = true;
                    }
                    break;
                }

                if (pageSet) continue;
            
                SetPageToUnknownFish(species);
                pageSet = true;
            }
        
            collection.text = $"{_discoveries.Count} / {speciesDb.Species.Count} Fish Collected";
        }

        public void SetPageToUnknownFish(FishSpecies species)
        {
            fishImage.sprite = species.Icon;
            fishImage.color = Color.black;
            title.text = "???";
            description.text = "";
            weightRange.text = "??? - ??? KG";
            catchHighscore.text = "???";
        }

        public void SetPageToFish(Fish fish)
        {
            fishImage.sprite = fish.Species.Icon;
            fishImage.color = Color.white;
            title.text = $"{fish.Species.Name}";
            description.text = fish.Species.Description;
            weightRange.text = $"{fish.Species.Weight} KG";

            catchHighscore.text = $"{fish.Weight:F1} KG";
        }

        public void LoadData(GameData data)
        {
            _discoveries = data.Discoveries.ToDictionary(speciesDb);
            Init();
        }

        public void SaveData(ref GameData data)
        {
            data.Discoveries = _discoveries.ToSerializableDictionary();
        }
    }
}
