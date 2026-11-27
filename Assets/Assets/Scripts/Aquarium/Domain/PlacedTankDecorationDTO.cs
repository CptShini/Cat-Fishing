using System;
using Assets.Scripts.Data.Domain.TankDecor;
using UnityEngine;



namespace Assets.Scripts.Aquarium.Domain
{
    [Serializable]
    public class PlacedTankDecorationDTO
    {
        public TankDecorationDTO tankDecoration;
        public int PlacementIndex;

        public PlacedTankDecorationDTO(PlacedTankDecoration placedTankDecoration)
        {
            this.tankDecoration = new TankDecorationDTO(placedTankDecoration.TankDecoration);
            this.PlacementIndex = placedTankDecoration.PlacementIndex;
        }
    }
}