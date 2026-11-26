using System;
using Assets.Scripts.Data.Domain.TankDecor;
using UnityEngine;



namespace Assets.Scripts.Aquarium.Domain
{

    public class PlacedTankDecoration
    {
        public Guid Id;
        public readonly TankDecoration TankDecoration;
        public int PlacementIndex;

        public PlacedTankDecoration(TankDecoration tankDecoration, int placementIndex)
        {
            TankDecoration = tankDecoration;
            PlacementIndex = placementIndex;
        }

        public PlacedTankDecoration(PlacedTankDecorationDTO placedTankDecorationDTO, TankDecorationTypeDatabase db)
        {
            TankDecoration = new(placedTankDecorationDTO.tankDecoration, db);
            PlacementIndex = placedTankDecorationDTO.PlacementIndex;
        }

    }

}