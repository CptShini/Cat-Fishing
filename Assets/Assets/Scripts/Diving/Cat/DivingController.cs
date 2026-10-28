using System;
using Assets.Scripts.PlayerState;
using UnityEngine;

using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;



public class DivingController : MonoBehaviour
{
    private DivingPlayerStats stats;

    [Header("Presence in Radius")]
    public float defaultPresence;
    public float stealthPresence;
    public float pouncePresence;
    
    [Header("Other")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CircleCollider2D presenceCollider; // collider for presence detection from fish
    [SerializeField] private Collider2D defaultCollider; // collider for obstacles
    [SerializeField] private StatBar staminaBar;
    [SerializeField] private StatBar oxygenBar;

    private FishCatCommunicator curFish;
    private float curStamina, curPounceStamina = 0; // currrent player stamina, and current stamina for pounce
    private float pounceDuration = 0; // Tracker for when to stop pounceing. Can change based on 
    private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
    private float curOxygen = 100;
    private float curContest = 0; // Tracking for a contest between fish and cat during catching
    private float contestThreshold = 5; // The value that contest must be to catch a fish succesfully
    
    private float curStunTimer = 0;
    private float curPounceCooldown;

    //[SerializeField] private AnimationCurve accelerationCurve;
    private Rigidbody2D playerBody;
    private Vector2 velocityVector;
    private PlayerState state;

    void Awake()
    {
        SetPlayerState(PlayerState.SWIM);
        stats = gameObject.GetComponent<DivingPlayerStats>(); 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetStamina(stats.maxStamina);
        SetOxygen(stats.oxygenCapacity);
        staminaBar.SetMaxValue(stats.maxStamina);
        oxygenBar.SetMaxValue(stats.oxygenCapacity);

        playerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnMove(CallbackContext  input)
    {
        velocityVector = input.ReadValue<Vector2>();
    }

    public void OnCancel(CallbackContext input)
    {
        if(state == PlayerState.STEALTH)
        {
            SetPlayerState(PlayerState.SWIM);
        }
    }

    public void OnStealth(CallbackContext  input)
    {
        if (input.performed) // Did the player press?
        {
            if(state == PlayerState.SWIM && curPounceCooldown <= 0 && !DivingInventory.Instance.isEnabled)
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

        float currentAcceleration = (velocityVector.x != 0) ? stats.acceleration : stats.friction;

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
            curPounceStamina + Time.deltaTime * stats.chargeUpSpeed, 
            0,
            curStamina < stats.maxPounceStamina ? curStamina : stats.maxPounceStamina);
    }
    public void StartPounce()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        SetPounceDirection(new Vector2(transform.position.x, transform.position.y), mousePos);

        pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now

        SetPlayerState(PlayerState.POUNCE);
        SetStamina(Mathf.Clamp(curStamina - curPounceStamina, 5, stats.maxStamina));
    }
    public void SetPounceDirection(Vector2 src, Vector2 dst)
    {
        pounceDirection = dst - src;
    }

    public void Pounce(Vector2 direction)
    {
        //Debug.Log(pounceSpeed + curPounceStamina * 0.5f);
        Vector2 targetVelocity = direction.normalized * (stats.pounceSpeed + curPounceStamina * 0.5f);

        playerBody.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed
    }

    void StaminaRegen(float amount)
    {
        SetStamina(Mathf.Clamp(curStamina + amount, 0, stats.maxStamina));
    }

    public void SetPlayerState(PlayerState state)
    {
        this.state = state;

        switch (state)
        {
            case PlayerState.STEALTH:
            {
                presenceCollider.radius = stealthPresence;
                curPounceStamina = 0;
                break;
            }
            case PlayerState.POUNCE:
            {
                presenceCollider.radius = pouncePresence;
                
                break;
            }
            case PlayerState.GRAB:
                {
                    presenceCollider.radius = pouncePresence;
                    curContest = contestThreshold / 2;
                    DivingInventory.Instance.SetEnabled(false);
                    break;
                }
            case PlayerState.STUN:
                {
                    curStunTimer = stats.stunDuration;
                    break;
                }
            default:
            {
                presenceCollider.radius = defaultPresence;
                break;
            }
        }
        
    }

    void Grab()
    {
        float angle = Vector2.Angle(velocityVector, curFish.direction);

        float power = stats.strength * (angle / 180);

        float contest = (power - curFish.strength) / curFish.strength;

        curContest += contest * Time.deltaTime;

        if( curContest >= contestThreshold)
        {
            //CATCH FISH
            curFish.joint.connectedBody = null;
            FishCollectible fish = new FishCollectible(curFish.fish);
            if(DivingInventory.Instance.Add(fish)) // Checks if putting in inventory is successful
            {
                Destroy(curFish.gameObject);
                SetPlayerState(PlayerState.SWIM);
            }
            else // Cat cannot win, if their inventory is full
            {
                SetPlayerState(PlayerState.STUN); 
            }

        } else if(curContest <= 0)
        {
            //LOSE FISH
            curFish.joint.connectedBody = null;
            SetPlayerState(PlayerState.STUN);
        }
        Debug.Log($"Contest: {curContest / contestThreshold * 100}%");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Disabled for now
        if (col.gameObject.layer == 10 && state != PlayerState.GRAB)
        {
            curFish = col.gameObject.GetComponent<FishCatCommunicator>();

            curFish.joint.connectedBody = this.playerBody;
            SetPlayerState(PlayerState.GRAB);
        }
        
    }
    
    void ReduceOxygen(float amount)
    {
        SetOxygen(curOxygen -= amount);
        //Debug.Log($"Reducing oxygen to: {curOxygen}");
        
        if(curOxygen < 0)
        {
            curOxygen = float.MaxValue;
            SceneTransition.Instance.LoadScene("AquariumScene");
        }
    }

    public void OnToggle(CallbackContext  input)
    {
        if( !(state == PlayerState.GRAB))
        {
            DivingInventory.Instance.Toggle();
        }
    }

    void Stunned()
    {
        curStunTimer -= Time.deltaTime;

        if (curStunTimer < 0)
        {
            defaultCollider.enabled = true;
            SetPlayerState(PlayerState.SWIM);
        }
    }

    void SetStamina(float value)
    {
        this.curStamina = value;
        staminaBar.SetCurrentValue(value);
    }

    void SetOxygen(float value)
    {
        this.curOxygen = value;
        oxygenBar.SetCurrentValue(value);
    }
    
    // Update is called once per frame
    void Update()
    {

        if (state != PlayerState.GRAB) // You cannot lose oxygen while battling a fish
        {
            ReduceOxygen(Time.deltaTime);
            curPounceCooldown -= Time.deltaTime;
        }
        
        //Debug.Log($"STAMINA: {curStamina}, CURPOUNCESTAMINA: {curPounceStamina}");
        switch (state)
        {
            case PlayerState.SWIM:
            {
                Swim(stats.speed);
                StaminaRegen(stats.staminaRegen * Time.deltaTime);

                break;
            }
            case PlayerState.STEALTH:
            {
                Swim(stats.stealthSpeed);
                ChargeUpPounce();
                break;
            }
            case PlayerState.POUNCE:
            {
                Pounce(pounceDirection);
                pounceDuration -= Time.deltaTime;

                if(pounceDuration < 0)
                    {
                        curPounceCooldown = stats.pounceCooldown; // start cooldown
                        SetPlayerState(PlayerState.SWIM);
                    }

                break;
            }
            case PlayerState.GRAB:
            {
                Grab();
                break;
            }
            case PlayerState.STUN:
                {
                    defaultCollider.enabled = false;
                    Swim(0);
                    Stunned();
                    break;
                }
            
            default:
            {
                break;
            }
        } 
    }


}
