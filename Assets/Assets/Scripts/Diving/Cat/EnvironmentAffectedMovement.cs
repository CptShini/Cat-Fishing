using System.Collections.Generic;
using Assets.Scripts.Diving.Hazards;
using Assets.Scripts.PlayerState;
using UnityEngine;

public class EnvironmentAffectedMovement : MonoBehaviour, IEnvironmentAffectedMovement
{

    private List<EnvironmentalMovementAffector> affectors = new();

    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void AddAffector(EnvironmentalMovementAffector affector)
    {
        affectors.Add(affector);
    }

    public void RemoveAffector(EnvironmentalMovementAffector affector)
    {
        affectors.Remove(affector);
    }

    public Vector2 CalculateVelocityChange()
    {
        Vector2 velocity = new();
        foreach (var affector in affectors)
        {
            velocity += affector.Velocity;
        }
        return velocity;
    }

    void FixedUpdate()
    {
        if(DivingController.Instance.state == PlayerState.GRAB) return;

        CalculateVelocityChange();
    }

}
