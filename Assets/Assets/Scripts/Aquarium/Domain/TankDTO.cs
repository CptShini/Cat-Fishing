using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Aquarium.Domain
{
    [Serializable]
    public class TankDTO
    {
        public string Id;
        public string Name;
        public int Capacity;
        public int UpgradeLevel;
        public float Dirtiness;
        public byte[] DirtTextureAlpha;
        public List<TankFishDTO> Fishes;
        public int[] FishDeathCount;

        public TankDTO(Tank tank)
        {
            Id = tank.Id.ToString();
            Name = tank.Name;
            Capacity = tank.Capacity;
            UpgradeLevel = tank.UpgradeLevel;
            Dirtiness = tank.Dirtiness;
            DirtTextureAlpha = tank.GetDirtTextureAlpha();
            Fishes = tank.Fishes.Select(f => new TankFishDTO(f)).ToList();
            FishDeathCount = tank.FishDeathCount.ToArray();
        }
    }
}