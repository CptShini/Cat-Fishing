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
        public int DecorCapacity;
        public int UpgradeLevel;
        public float Dirtiness;
        public List<TankFishDTO> Fishes;
        public List<PlacedTankDecorationDTO> Decorations;
        public int[] FishDeathCount;

        public TankDTO(Tank tank)
        {
            Id = tank.Id.ToString();
            Name = tank.Name;
            Capacity = tank.Capacity;
            DecorCapacity = tank.DecorCapacity;
            UpgradeLevel = tank.UpgradeLevel;
            Dirtiness = tank.Dirtiness;
            Fishes = tank.Fishes.Select(f => new TankFishDTO(f)).ToList();
            Decorations = tank.Decorations.Select(f => 
                new PlacedTankDecorationDTO(f)).ToList();
            FishDeathCount = tank.FishDeathCount.ToArray();
        }
    }
}