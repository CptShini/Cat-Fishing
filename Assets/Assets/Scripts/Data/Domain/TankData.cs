using System;
using System.Collections.Generic;
using Assets.Scripts.Fishies.Domain;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class TankData
    {
        public string TankId;
        public List<FishDTO> Fishes;

        public TankData()
        {
            Fishes = new();
        }

        public TankData(string tankId, List<FishDTO> fishes)
        {
            TankId = tankId;
            Fishes = fishes;
        }
    }
}