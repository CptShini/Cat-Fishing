using System;
using System.Collections.Generic;
using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Data.Domain.Upgrades;
using Assets.Scripts.Fishies.Domain;
using static Assets.Scripts.Data.Domain.Upgrades.LevelUpgrade;
using static Assets.Scripts.Data.Domain.Upgrades.OneTimeUpgrade;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public bool DayPassed = true;
        
        public int Gold = 10;
        
        public float CrowdScore;
        public int[] CrowdHistory = new int[5];
        
        public SerializableDictionary<EntertainmentContributor, float> EntertainmentValues = new()
        {
            { EntertainmentContributor.Tank, 0f },
            { EntertainmentContributor.Decor, 0f }
        };
        
        public SerializableDictionary<string, TankFishDTO> Collection = new();
        public SerializableDictionary<string, TankDecorationDTO> TankDecorations = new();
        public SerializableDictionary<string, TankDecorationDTO> DailyTankDecorations = new(); // Gets cleared after daily report
        public SerializableDictionary<string, TankDTO> Tanks = new();
        
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