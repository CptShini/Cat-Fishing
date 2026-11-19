
using System;
using UnityEngine;
namespace Assets.Scripts.Data.Domain.TankDecor
{ 
    public class TankDecorationDTO
    {
        public string Id;
        public int TypeId;

        public TankDecorationDTO(TankDecoration decoration)
        {
            Id = decoration.Id.ToString();
            TypeId = decoration.type.Id;
        }
    }
}
