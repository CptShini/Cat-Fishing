using System;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public SerializableDictionary<string, FishDTO> Collection;
        public SerializableDictionary<string, TankData> Tanks;
        
        public GameData()
        {
            Collection = new();
            Tanks = new();
        }
    }
}