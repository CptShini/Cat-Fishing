using System;
using System.Collections.Generic;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Upgrades;
using static Assets.Scripts.Upgrades.LevelUpgrade;
using static Assets.Scripts.Upgrades.OneTimeUpgrade;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public DecorationSaveData Decoration = new();
        public SerializableDictionary<string, FishDTO> Collection = new();
        public SerializableDictionary<string, TankData> Tanks = new();
        public SerializableDictionary<string, FishDTO> Discoveries = new();
        public List<int> ExploredFogs = new();
        
        // Shop
        public int Gold;
        
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
    }
}