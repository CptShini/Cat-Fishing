using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Upgrades;
using static Assets.Scripts.Upgrades.LevelUpgrade;
using static Assets.Scripts.Upgrades.OneTimeUpgrade;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public int Gold = 200;
        public int CurrentCrowd;
        public bool DayPassed = true;
        
        public SerializableDictionary<string, TankFishDTO> Collection = new();
        public SerializableDictionary<string, TankDTO> Tanks = new()
        {
            { "07679c15-0847-4c65-abb5-aa04b8adc8ac", new(new("Fish Tank 1", 4))},
            { "0b5546b1-f057-4199-8716-cf68abdcebdf", new(new("Fish Tank 2", 4))}
        };
        
        public SerializableDictionary<OneTimeUpgrade, bool> OneTimeUpgrades = new()
        {
            { Minimap, false }
        };
        
        public SerializableDictionary<LevelUpgrade, int> LevelUpgrades = new()
        {
            { Speed, 0 },
            { PounceSpeed, 0 },
            { ClawStrength, 0 },
            { OxygenCapacity, 0 },
            { CarryingCapacity, 0 }
        };
        
        public SerializableDictionary<string, FishDTO> Discoveries = new();
        public List<int> ExploredFogs = new();
        
        public DecorationSaveData Decoration = new();
    }
}