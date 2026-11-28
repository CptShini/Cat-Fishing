using System;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.Data.Domain.TankDecor
{
    
    public class TankDecoration
    {
        public readonly Guid Id;

        public TankDecorationType Type;

        public TankDecoration(TankDecorationType type)
        {
            Id = Guid.NewGuid();
            Type = type;
        }

        public TankDecoration(TankDecorationDTO tankDecorDTO, TankDecorationTypeDatabase db)
        {
            Id = tankDecorDTO.Id.TryParseGuid();
            Type = db[tankDecorDTO.TypeId];
        }
    
    }
}