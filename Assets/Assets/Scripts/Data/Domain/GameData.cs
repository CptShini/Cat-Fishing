using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Domain.Aquarium;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class GameData
    {
        public SerializableDictionary<string, FishDTO> Collection = new();
        public SerializableDictionary<string, TankDTO> Tanks = new();
        public SerializableDictionary<string, FishDTO> Discoveries = new();
        public List<int> ExploredFogs = new();
    }
}