using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Aquarium.Domain
{
    public class Tank
    {
        public readonly Guid Id;
        public string Name;
        
        public int Capacity { get; private set; }
        
        public int Dirtiness { get; private set; }
        
        public float Entertainment { get; private set; }
        
        public IReadOnlyList<TankFish> Fishes => _fishes;
        private readonly List<TankFish> _fishes;

        public IReadOnlyList<int> FishDeathCount => _fishDeathCount;
        private int[] _fishDeathCount;

        private readonly List<TankFish> _deadFish = new();

        public Tank(Guid id, string name, int capacity)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            Dirtiness = 0;
            Entertainment = 0f;
            _fishes = new();
            _fishDeathCount = new int[3];
        }

        public Tank(TankDTO tankDTO, FishSpeciesDatabase db)
        {
            Id = Guid.Parse(tankDTO.Id);
            Name = tankDTO.Name;
            Capacity = tankDTO.Capacity;
            Dirtiness = tankDTO.Dirtiness;
            
            _fishes = tankDTO.Fishes.Select(dto => new TankFish(dto, db)).ToList();
            _fishes.ForEach(RegisterFish);
            OnFishChanged();

            _fishDeathCount = tankDTO.FishDeathCount;
        }
        
        public void DayStarted()
        {
            _fishes.ForEach(f => f.DayStarted());
            RecalculateEntertainment();
            
            if (_fishes.Count > 0 && Dirtiness < 10) Dirtiness++;
            
            _fishDeathCount = new int[3];
            _deadFish.Clear();
        }

        public void DayEnded()
        {
            if (Dirtiness == 10)
                _fishes.ForEach(f => f.TankTooDirty());
            else
                _fishes.ForEach(f => f.DayEnded());

            _deadFish.ForEach(df => _fishes.Remove(df));
            OnFishChanged();
        }

        public void AddFish(TankFish tankFish)
        {
            _fishes.Add(tankFish);
            RegisterFish(tankFish);
            OnFishChanged();
        }

        public void RemoveFish(TankFish tankFish)
        {
            _fishes.Remove(tankFish);
            UnregisterFish(tankFish);
            OnFishChanged();
        }

        private void RegisterFish(TankFish fish)
        {
            fish.Fed.AddListener(RecalculateEntertainment);
            fish.Died.AddListener(FishDied);
        }

        private void UnregisterFish(TankFish fish)
        {
            fish.Fed.RemoveListener(RecalculateEntertainment);
            fish.Died.RemoveListener(FishDied);
        }

        private void FishDied(TankFish fish, FishCauseOfDeath cod)
        {
            Debug.Log($"{fish.Fish.Species.Name} died from {cod}");
            _fishDeathCount[(int)cod]++;
            _deadFish.Add(fish);
            UnregisterFish(fish);
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
        
        public void CleanTank() =>
            Dirtiness = 0;
    }
}