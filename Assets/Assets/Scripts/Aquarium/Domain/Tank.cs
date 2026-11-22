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
        public int UpgradeLevel { get; private set; }

        public const float MaxDirtiness = 6f;
        private float _dirtiness, _dirtinessFactor;
        public float Dirtiness
        {
            get => _dirtiness;
            set
            {
                _dirtiness = Mathf.Clamp(value, 0f, MaxDirtiness);
                
                const float threshold = 1f;
                if (Dirtiness <= threshold)
                {
                    _dirtinessFactor = 1f;
                    return;
                }
                
                float x = (Dirtiness - threshold) / (MaxDirtiness - threshold);
                _dirtinessFactor = 1f - x*x;
            }
        }

        private byte[] _dirtTextureAlpha;
        public byte[] DirtTextureAlpha
        {
            get => (byte[])_dirtTextureAlpha?.Clone();
            set => _dirtTextureAlpha = (byte[])value.Clone();
        }

        private float _entertainmentValue;
        public float Entertainment => _entertainmentValue * _dirtinessFactor;

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
            UpgradeLevel = 1;
            Dirtiness = 0;
            _entertainmentValue = 0f;
            _fishes = new();
            _fishDeathCount = new int[3];
        }

        public Tank(TankDTO tankDTO, FishSpeciesDatabase db)
        {
            Id = Guid.Parse(tankDTO.Id);
            Name = tankDTO.Name;
            Capacity = tankDTO.Capacity;
            UpgradeLevel = tankDTO.UpgradeLevel <= 0 ? 1 : tankDTO.UpgradeLevel;
            Dirtiness = tankDTO.Dirtiness;
            _dirtTextureAlpha = (byte[])tankDTO.DirtTextureAlpha?.Clone();
            
            _fishes = tankDTO.Fishes.Select(dto => new TankFish(dto, db)).ToList();
            _fishes.ForEach(RegisterFish);
            OnFishChanged();

            _fishDeathCount = tankDTO.FishDeathCount;
        }
        
        public void DayStarted()
        {
            _dirtTextureAlpha = null;
            
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
            
            _entertainmentValue = 0f;
            foreach (var fish in _fishes.OrderByDescending(f => f.Entertainment))
            {
                int speciesId = fish.Fish.Species.Id;
                speciesCount.TryAdd(speciesId, 0);
                speciesCount[speciesId]++;

                float noveltyFactor = Mathf.Pow(2f, 1 - speciesCount[speciesId]);
                _entertainmentValue += fish.Entertainment * noveltyFactor;
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