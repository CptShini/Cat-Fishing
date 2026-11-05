using Assets.Scripts.Diving.Cat;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;



public class DivingController : MonoBehaviour
{
    public static DivingController Instance { get; private set; }

    private DivingPlayerStats stats;
    
    [Header("Other")]
    [SerializeField] private CircleCollider2D presenceCollider; // collider for presence detection from fish
    [SerializeField] private Collider2D defaultCollider; // collider for obstacles

    public PlayerState state;
    private PounceScript _pounce;
    private FishCatCommunicator curFish;
    private float curStunTimer = 0;

    private Rigidbody2D playerBody;
    private Vector2 velocityVector;

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = DivingPlayerStats.Instance;
        _pounce = gameObject.GetComponent<PounceScript>();
        SetPlayerState(PlayerState.SWIM);
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
            if(state == PlayerState.SWIM && _pounce.curPounceCooldown <= 0 && !DivingInventory.Instance.IsEnabled)
            {
                SetPlayerState(PlayerState.STEALTH);
            }
        }
        else if (input.canceled) // Did the player release
        {
            if(state == PlayerState.STEALTH)
            {
                _pounce.StartPounce();
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

    public void SetPlayerState(PlayerState state)
    {
        this.state = state;

        switch (state)
        {
            case PlayerState.STEALTH:
            {
                presenceCollider.radius = stats.stealthPresence;
                _pounce.curPounceStamina = 0;
                break;
            }
            case PlayerState.POUNCE:
            {
                presenceCollider.radius = stats.pouncePresence;     
                break;
            }
            case PlayerState.GRAB:
                {
                    presenceCollider.radius = stats.pouncePresence;
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
                presenceCollider.radius = stats.defaultPresence;
                break;
            }
        } 
    }

    void Grab()
    {
        playerBody.linearVelocity = new Vector3(0, 0, 0);
        CatchingMinigame.Instance.Grab(velocityVector);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer != 7 || state == PlayerState.GRAB) return;
        
        Fish fish = col.gameObject.GetComponent<FishController>().Fish;
        FishCaughtMovement movement = col.gameObject.GetComponent<FishCaughtMovement>();

        col.gameObject.GetComponent<FishController>().Catch();
        CatchingMinigame.Instance.Init(fish, movement);
        SetPlayerState(PlayerState.GRAB);
    }


    public void OnToggle(CallbackContext  input)
    {
        if(state == PlayerState.GRAB)
        {
            return;
        }
        DivingInventory.Instance.Toggle();
    }

    void Stunned()
    {
        curStunTimer -= Time.deltaTime;
        if (curStunTimer >= 0)
        {
            return;
        }
        defaultCollider.enabled = true;
        SetPlayerState(PlayerState.SWIM);
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.SWIM:
            {
                Swim(stats.speed);
                break;
            }
            case PlayerState.STEALTH:
            {
                Swim(stats.stealthSpeed);
                _pounce.ChargeUpPounce();
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
