using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FishWanderMovement))]
[RequireComponent(typeof(FishCaughtMovement))]
public class FishController : MonoBehaviour
{
    [SerializeField] private float 
        _minWanderTime = 1f,
        _maxWanderTime = 15f,
        _minFleeTime = 5f,
        _maxFleeTime = 10f;
    
    private FishState _state;
    private FishSpeed _speedState;
    
    private FishWanderMovement _wanderMovement;
    private FishCaughtMovement _caughtMovement;

    private void Start()
    {
        _wanderMovement = GetComponent<FishWanderMovement>();
        _caughtMovement = GetComponent<FishCaughtMovement>();
        
        Wander();
    }

    private void Update()
    {
        // Sensor
    }

    private void Wander()
    {
        _state = FishState.Wander;
        _speedState = (FishSpeed)Random.Range(0, 3); // TODO: weighted random?

        _wanderMovement.enabled = true;
        _caughtMovement.enabled = false;
        
        _wanderMovement.Wander(_speedState);
        
        float wanderDuration = Random.Range(_minWanderTime, _maxWanderTime);
        StartCoroutine(nameof(WanderAfterDelay), wanderDuration);
    }
    
    private void Flee()
    {
        _state = FishState.Flee;
        _speedState = FishSpeed.Max;
        
        _wanderMovement.enabled = true;
        _caughtMovement.enabled = false;
        
        // TODO: custom implementation instead
        _wanderMovement.Wander(_speedState);
        
        float fleeDuration = Random.Range(_minFleeTime, _maxFleeTime);
        StartCoroutine(nameof(WanderAfterDelay), fleeDuration);
    }

    private void Caught()
    {
        _state = FishState.Caught;
        
        _wanderMovement.enabled = false;
        _caughtMovement.enabled = true;
        
        // TODO
    }

    private IEnumerator WanderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Wander();
    }
}