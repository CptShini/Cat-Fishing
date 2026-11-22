
using Assets.Scripts.Diving.Hazards;
using UnityEngine;

public interface IEnvironmentAffectedMovement
{
    void AddAffector(EnvironmentalMovementAffector affector);

    void RemoveAffector(EnvironmentalMovementAffector affector);

    Vector2 CalculateVelocityChange();

}

