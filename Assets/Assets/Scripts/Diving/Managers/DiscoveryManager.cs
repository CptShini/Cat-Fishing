using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain;
using Assets.Scripts.Data.Persistence;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Diving.Managers
{
    public class DiscoveryManager : MonoBehaviour, IDataPersistence
    {
        public readonly UnityEvent<Fish> Discovered = new();
        public readonly UnityEvent<Fish> NewRecord = new();
    
        [SerializeField] private FishSpeciesDatabase species;

        private Dictionary<Guid, Fish> _discoveries;

        private void Start() => 
            CatchingMinigame.Instance.OnWin.AddListener(CheckForDiscovery);

        private void CheckForDiscovery(Fish fish)
        {
            if (_discoveries.Count == 0)
            {
                NewDiscovery(fish);
                return;
            }
        
            foreach (Fish discoveredFish in _discoveries.Values.Where(discoveredFish => fish.Species == discoveredFish.Species))
            {
                if (fish.Weight > discoveredFish.Weight)
                {
                    UpdateRecord(fish, discoveredFish);
                }
            
                return;
            }
        
            NewDiscovery(fish);
        }

        private void UpdateRecord(Fish fish, Fish other)
        {
            _discoveries.Remove(other.Id);
            _discoveries.Add(fish.Id, fish);
            NewRecord.Invoke(fish);
        }

        private void NewDiscovery(Fish fish)
        {
            _discoveries.Add(fish.Id, fish);
            Discovered.Invoke(fish);
        }

        public void LoadData(GameData data)
        {
            _discoveries = data.Discoveries.ToDictionary(dto => new Fish(dto, species));
        }

        public void SaveData(ref GameData data)
        {
            data.Discoveries = _discoveries.ToSerializableDictionary(fish => new FishDTO(fish));
        }

    }
}
