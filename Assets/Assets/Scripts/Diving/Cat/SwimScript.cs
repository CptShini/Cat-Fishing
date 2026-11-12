using Assets.Scripts.Audio;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;



public class SwimScript : MonoBehaviour
{

    [SerializeField] Animator animator;
    private float swimSpeed;
    private float friction;
    private float acceleration;
    public readonly UnityEvent<Vector2, float> OnSwim = new();
    public readonly UnityEvent<Vector2> OnStealth = new();

    private Vector2 velocityVector;

    private bool swimming, stealth;

    private Rigidbody2D _rb;
    
    private void Start()
    {
        swimSpeed = DivingPlayerStats.Instance.speed;
        friction = DivingPlayerStats.Instance.friction;
        acceleration = DivingPlayerStats.Instance.acceleration;
        DivingController.Instance.OnStateChanged.AddListener(HandlePlayerStateChanged);
        HandlePlayerStateChanged(DivingController.Instance.state);
        _rb = GetComponent<Rigidbody2D>();
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
        if (stealth)
        {
            OnStealth.Invoke(direction);
        }
        float currentAcceleration = (direction.x != 0) ? acceleration : friction;

        Vector2 newVelocity = Vector2.MoveTowards(
            _rb.linearVelocity,
            targetVelocity,
            currentAcceleration * Time.deltaTime
        ); 
        _rb.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);

        // Play swimming audio only when actually moving
        if (!stealth && _rb.linearVelocity.magnitude > 0.1f)
        {
            AudioManager.Instance.Play(AudioId.CatSwim);
            bool isSwimming = true;
            float speed = 1f;
            animator.SetBool("swimming", isSwimming);
            animator.SetFloat("speed", speed);
        }
        else
        {
            bool isSwimming = true;
            float speed = .4f;
            animator.SetBool("swimming", isSwimming);
            animator.SetFloat("speed", speed);
            AudioManager.Instance.FadeOutLoop(AudioId.CatSwim);
        }
    }

    private void FixedUpdate()
    {
        if (swimming)
        {
            Swim(velocityVector);
        }
    }
}
