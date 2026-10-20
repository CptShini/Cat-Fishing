using System;
using UnityEngine;


using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum PlayerState
{
    SWIM, // Default state
    STEALTH,
    POUNCE,
}

public class DivingController : MonoBehaviour
{

    [Header("Speed")]
    public float speed = 10;
    public float stealthSpeed = 3;
    public float pounceSpeed = 20;

    [Header("Acceleration")]
    public float acceleration = 5; 
    public float friction = 5;
    
    [Header("Stamina")]
    public float maxStamina = 60, maxPounceStamina = 20;
    public float staminaRegen = 5;
    
    [Header("Charge Up")]
    public float chargeUpSpeed = 10;  // per second

    [Header("Presence in Radius")]
    public float defaultPresence;
    public float stealthPresence;
    public float puoncePresence;
    
    [Header("Other")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CircleCollider2D presenceCollider; // collider for presence detection from fish
    [SerializeField] private Collider2D defaultCollider; // collider for obstacles



    private float curStamina, curPounceStamina = 0; // currrent player stamina, and current stamina for pounce
    private float pounceDuration = 0; // Tracker for when to stop pounceing. Can change based on 
    private Vector2 pounceDirection; // For saving the pounce direction during multiple updates

    //[SerializeField] private AnimationCurve accelerationCurve;
    private Rigidbody2D playerBody;
    private Vector2 velocityVector;
    private PlayerState state;

    void Awake()
    {
        SetPlayerState(PlayerState.SWIM);
        curStamina = maxStamina;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnMove(CallbackContext  input)
    {
        velocityVector = input.ReadValue<Vector2>();
    }

    public void OnStealth(CallbackContext  input)
    {
        if (input.performed) // Did the player press?
        {
            if(state == PlayerState.SWIM)
            {
                SetPlayerState(PlayerState.STEALTH);
            }
        }
        else if (input.canceled) // Did the player release
        {

            if(state == PlayerState.STEALTH)
            {
                StartPounce();
            }
        }
    }

    public void Swim(float swimSpeed)
    {
        Vector2 targetVelocity = velocityVector.normalized * swimSpeed;

        float currentAcceleration = (velocityVector.x != 0) ? acceleration : friction;

        Vector2 newVelocity = Vector2.MoveTowards(
            playerBody.linearVelocity,
            targetVelocity,
            currentAcceleration * Time.deltaTime
        );
        
        playerBody.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);
    }

    public void ChargeUpPounce()
    {
        curPounceStamina = Mathf.Clamp(
            curPounceStamina + Time.deltaTime * chargeUpSpeed, 
            0,
            curStamina < maxPounceStamina ? curStamina : maxPounceStamina);
    }
    public void StartPounce()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        SetPounceDirection(new Vector2(transform.position.x, transform.position.y), mousePos);

        pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now

        SetPlayerState(PlayerState.POUNCE);

        curStamina = Mathf.Clamp(curStamina - curPounceStamina, 5, maxStamina);
        curPounceStamina = 0;
    }
    public void SetPounceDirection(Vector2 src, Vector2 dst)
    {
        pounceDirection = dst - src;
    }

    public void Pounce(Vector2 direction)
    {
        Vector2 targetVelocity = direction.normalized * pounceSpeed;

        playerBody.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed
    }

    void StaminaRegen(float amount)
    {
        curStamina = Mathf.Clamp(curStamina + amount, 0, maxStamina);
    }

    public void SetPlayerState(PlayerState state)
    {
        this.state = state;

        switch (state)
        {
            case PlayerState.STEALTH:
            {
                presenceCollider.radius = stealthPresence;
                break;
            }
            case PlayerState.POUNCE:
            {
                presenceCollider.radius = puoncePresence;
                break;
            }
            default:
            {
                presenceCollider.radius = defaultPresence;
                break;
            }
        }
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("AHHHHHH");
    }
    
    

  
    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log($"STAMINA: {curStamina}, CURPOUNCESTAMINA: {curPounceStamina}");
        switch (state)
        {
            case PlayerState.SWIM:
            {
                Swim(speed);
                StaminaRegen(staminaRegen * Time.deltaTime);

                break;
            }
            case PlayerState.STEALTH:
            {
                Swim(stealthSpeed);
                ChargeUpPounce();
                break;
            }
            case PlayerState.POUNCE:
            {
                Pounce(pounceDirection);
                pounceDuration -= Time.deltaTime;

                if(pounceDuration < 0)
                    {
                        SetPlayerState(PlayerState.SWIM);
                    }

                break;
            }
            
            default:
            {
                break;
            }
        } 
    }


}
