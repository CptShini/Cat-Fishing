using System;
using System.Collections.Generic;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public SerializableDictionary<string, FishDTO> Collection;
        public SerializableDictionary<string, TankData> Tanks;
        public List<int> ExploredFogs;
        
        public GameData()
        {
            Collection = new();
            Tanks = new();
            ExploredFogs = new();
        }
    }
}