using System;

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
            Id = Guid.Parse(tankDecorDTO.Id);
            Type = db[tankDecorDTO.TypeId];
        }
    
    }
}