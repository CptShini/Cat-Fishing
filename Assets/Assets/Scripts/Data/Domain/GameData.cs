using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public DecorationSaveData Decoration;
        public SerializableDictionary<string, FishDTO> Collection;
        public SerializableDictionary<string, TankData> Tanks;
        public SerializableDictionary<string, float> PlayerStats;
        public int Gold;

        public GameData()
        {
            Decoration = new DecorationSaveData();
            Collection = new();
            Tanks = new();

            Gold = 0;
            
            PlayerStats = new()
            {
                { "Speed", 10f },
                { "PounceSpeed", 20f },
                { "ClawsStrength", 10f },
                { "OxygenCapacity", 100f },
                { "CarryingCapacity", 25f },

                { "SpeedLevel", 0f },
                { "PounceSpeedLevel", 0f },
                { "ClawsStrengthLevel", 0f },
                { "OxygenCapacityLevel", 0f },
                { "CarryingCapacityLevel", 0f },

                { "HasMiniMap", 0}
            };
        }
    }
}