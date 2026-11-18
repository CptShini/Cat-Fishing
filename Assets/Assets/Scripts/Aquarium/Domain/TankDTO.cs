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
        public List<TankFishDTO> Fishes;

        public TankDTO(Tank tank)
        {
            Id = tank.Id.ToString();
            Name = tank.Name;
            Capacity = tank.Capacity;
            Fishes = tank.Fishes.Select(f => new TankFishDTO(f)).ToList();
        }
    }
}