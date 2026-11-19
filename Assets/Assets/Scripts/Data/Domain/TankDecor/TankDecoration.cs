using System;
using UnityEngine;
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
    
    }
}