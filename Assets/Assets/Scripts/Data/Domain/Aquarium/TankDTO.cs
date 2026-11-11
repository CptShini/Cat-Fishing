using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Data.Domain.Aquarium
{
    [Serializable]
    public class TankDTO
    {
        public string Id;
        public int Capacity;
        public List<TankFishDTO> Fishes;

        public TankDTO(Tank tank)
        {
            Id = tank.Id.ToString();
            Capacity = tank.Capacity;
            Fishes = tank.Fishes.Select(f => new TankFishDTO(f)).ToList();
        }
    }
}