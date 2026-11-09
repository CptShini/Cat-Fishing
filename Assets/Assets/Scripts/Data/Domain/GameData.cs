using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public SerializableDictionary<string, FishDTO> Collection;
        public SerializableDictionary<string, TankData> Tanks;
        public SerializableDictionary<string, float> PlayerStats;

        public GameData()
        {
            Collection = new();
            Tanks = new();

            PlayerStats = new()
            {
                { "Speed", 10f },
                { "PounceSpeed", 20f },
                { "ClawsStrength", 10f },
                { "OxygenCapacity", 100f },
                { "CarryingCapacity", 25f },
                { "HasMiniMap", 0}
            };
        }
    }
}