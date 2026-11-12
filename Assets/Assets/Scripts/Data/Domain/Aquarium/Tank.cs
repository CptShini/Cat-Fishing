using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Data.Domain.Aquarium
{
    public class Tank
    {
        public readonly Guid Id;
        public string Name;
        
        public int Capacity { get; private set; }
        
        public float Entertainment { get; private set; }
        
        public IReadOnlyList<TankFish> Fishes => _fishes;
        private readonly List<TankFish> _fishes;

        public Tank(string name, int capacity)
        {
            Id = Guid.NewGuid();
            Name = name;
            Capacity = capacity;
            Entertainment = 0f;
            _fishes = new();
        }

        public Tank(TankDTO tankDTO, FishSpeciesDatabase db)
        {
            Id = Guid.Parse(tankDTO.Id);
            Name = tankDTO.Name;
            Capacity = tankDTO.Capacity;
            _fishes = tankDTO.Fishes.Select(dto => new TankFish(dto, db)).ToList();
            OnFishChanged();
        }
        
        public void DayPassed() => _fishes.ForEach(f => f.DayPassed());
        public void Feed() => _fishes.ForEach(f => f.Feed());

        public void AddFish(TankFish tankFish)
        {
            _fishes.Add(tankFish);
            OnFishChanged();
        }

        public void RemoveFish(TankFish tankFish)
        {
            _fishes.Remove(tankFish);
            OnFishChanged();
        }

        private void OnFishChanged()
        {
            _fishes.ForEach(f => f.SetTankCrowdingInfo(_fishes.Count, Capacity));
            RecalculateEntertainment();
        }

        private void RecalculateEntertainment()
        {
            Dictionary<int, int> speciesCount = new();
            
            float entertainmentValue = 0f;
            foreach (var fish in _fishes.OrderByDescending(f => f.Entertainment))
            {
                int speciesId = fish.Fish.Species.Id;
                speciesCount.TryAdd(speciesId, 0);
                speciesCount[speciesId]++;

                float noveltyFactor = Mathf.Pow(2f, 1 - speciesCount[speciesId]);
                entertainmentValue += fish.Entertainment * noveltyFactor;
            }
            
            Entertainment = entertainmentValue;
        }
    }
}