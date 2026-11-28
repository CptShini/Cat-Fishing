using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Fishies.Domain;
using UnityEngine;

namespace Assets.Scripts.Aquarium.Domain
{
    public class Tank
    {
        public readonly Guid Id;
        public string Name;
        
        public int Capacity { get; private set; }
        public int DecorCapacity { get; private set; }
        public int UpgradeLevel { get; private set; }

        private const float MaxDirtiness = 6f, DirtinessThreshold = 1f;
        private float _dirtiness;
        public float DirtinessFactor { get; private set; }
        public float Dirtiness
        {
            get => _dirtiness;
            set
            {
                _dirtiness = Mathf.Clamp(value, 0f, MaxDirtiness);
                DirtinessFactor = Dirtiness <= DirtinessThreshold
                    ? 0f
                    : (Dirtiness - DirtinessThreshold) / (MaxDirtiness - DirtinessThreshold);
            }
        }

        private float _entertainmentValue;
        public float Entertainment => _entertainmentValue * (1f - DirtinessFactor*DirtinessFactor);

        public IReadOnlyList<TankFish> Fishes => _fishes;
        private readonly List<TankFish> _fishes;

        public IReadOnlyList<PlacedTankDecoration> Decorations => _decorations;
        private readonly List<PlacedTankDecoration> _decorations;

        public IReadOnlyList<int> FishDeathCount => _fishDeathCount;
        private int[] _fishDeathCount;

        private readonly List<TankFish> _deadFish = new();

        public Tank(Guid id, string name, int capacity, int decorCapacity)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            DecorCapacity = decorCapacity;
            UpgradeLevel = 1;
            Dirtiness = 0;
            _entertainmentValue = 0f;
            _fishes = new();
            _fishDeathCount = new int[3];
            _decorations = new();
        }

        public Tank(TankDTO tankDTO, FishSpeciesDatabase db, TankDecorationTypeDatabase dbDecoration)
        {
            Id = Guid.Parse(tankDTO.Id);
            Name = tankDTO.Name;
            Capacity = tankDTO.Capacity;
            DecorCapacity = tankDTO.DecorCapacity;
            UpgradeLevel = tankDTO.UpgradeLevel <= 0 ? 1 : tankDTO.UpgradeLevel;
            Dirtiness = tankDTO.Dirtiness;
            

            _fishes = tankDTO.Fishes.Select(dto => new TankFish(dto, db)).ToList();
            _fishes.ForEach(RegisterFish);

            _decorations = tankDTO.Decorations.Select(dto => new PlacedTankDecoration(dto, dbDecoration)).ToList();

            OnFishChanged();

            _fishDeathCount = tankDTO.FishDeathCount;
        }
        
        public void DayStarted()
        {
            _fishes.ForEach(f => f.DayStarted());
            RecalculateEntertainment();
            
            if (_fishes.Count > 0 && Dirtiness < MaxDirtiness) Dirtiness++;
            
            _fishDeathCount = new int[3];
            _deadFish.Clear();
        }

        public void DayEnded()
        {
            if (Dirtiness >= MaxDirtiness)
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

        public void AddDecoration(PlacedTankDecoration tankDecoration)
        {
            _decorations.Add(tankDecoration);
            RecalculateEntertainment();
        }

        public void RemoveDecoration(PlacedTankDecoration tankDecoration)
        {
            _decorations.Remove(tankDecoration);
            RecalculateEntertainment();
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
            
            _entertainmentValue = 0f;
            foreach (var fish in _fishes.OrderByDescending(f => f.Entertainment))
            {
                int speciesId = fish.Fish.Species.Id;
                speciesCount.TryAdd(speciesId, 0);
                speciesCount[speciesId]++;

                float noveltyFactor = Mathf.Pow(2f, 1 - speciesCount[speciesId]);
                _entertainmentValue += fish.Entertainment * noveltyFactor;
            }
            foreach (var tankDecoration in _decorations)
            {
                _entertainmentValue += tankDecoration.TankDecoration.Type.EntertainmentValue;
            }
        }

        public bool Upgrade(int newCapacity)
        {
            if (newCapacity <= Capacity)
                return false;

            Capacity = newCapacity;
            UpgradeLevel++;

            OnFishChanged();
            return true;
        }
    }
}