using System.Threading.Tasks;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;



public class SwimScript : MonoBehaviour
{

    private float swimSpeed = 0;
    private float friction = 0;
    private float acceleration = 0;
    public readonly UnityEvent<Vector2, float> OnSwim = new();
    private Vector2 velocityVector;

    bool swimming = false;
    bool stealth = false;

    private Rigidbody2D _pb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swimSpeed = DivingPlayerStats.Instance.speed;
        friction = DivingPlayerStats.Instance.friction;
        acceleration = DivingPlayerStats.Instance.acceleration;
        DivingController.Instance.OnStateChanged.AddListener(HandlePlayerStateChanged);
        HandlePlayerStateChanged(DivingController.Instance.state);
        _pb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void HandlePlayerStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.SWIM:
                swimSpeed = DivingPlayerStats.Instance.speed;
                stealth = false;
                break;
            case PlayerState.STEALTH:
                swimSpeed = DivingPlayerStats.Instance.stealthSpeed;
                stealth = true;
                break;
            case PlayerState.STUN:
                swimSpeed = 0;
                stealth = false;
                break; 
            default:
                swimming = false;
                return;
        }
        swimming = true;
    }

    public void OnMove(CallbackContext input)
    {
        velocityVector = input.ReadValue<Vector2>();
    }

    private void Swim(Vector2 direction)
    {
        Vector2 targetVelocity = direction.normalized * swimSpeed;
        if (!stealth && swimSpeed != 0)
        {
            OnSwim.Invoke(direction, 250);
        }
        float currentAcceleration = (direction.x != 0) ? acceleration : friction;

        Vector2 newVelocity = Vector2.MoveTowards(
            _pb.linearVelocity,
            targetVelocity,
            currentAcceleration * Time.deltaTime
        ); 
        _pb.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);

        // Play swimming audio only when actually moving
        if (!stealth && _pb.linearVelocity.magnitude > 0.1f)
        {
            AudioManager.Instance.PlayCatSwimming();
        }
        else
        {
            AudioManager.Instance.StopPlaying();
        }
    }

    void FixedUpdate()
    {
        if (swimming)
        {
            Swim(velocityVector);
        }
    }
}
