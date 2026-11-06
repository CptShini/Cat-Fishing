using Assets.Scripts.Diving.Cat;
using Assets.Scripts.Diving.DivingInventory;
using Assets.Scripts.Fishies;
using Assets.Scripts.Fishies.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;



public class DivingController : MonoBehaviour
{
    public static DivingController Instance { get; private set; }

    private DivingPlayerStats stats;
    
    [Header("Other")]
    [SerializeField] private CircleCollider2D presenceCollider; // collider for presence detection from fish
    [SerializeField] private Collider2D defaultCollider; // collider for obstacles

    public PlayerState state;
    [SerializeField] private PounceScript _pounce;
    private float curStunTimer = 0;

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private GameObject graphics;
    public readonly UnityEvent<PlayerState> OnStateChanged = new();

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
        SetPlayerState(PlayerState.SWIM);
    }


    public void OnCancel(CallbackContext input)
    {
        if(state == PlayerState.STEALTH)
        {
            SetPlayerState(PlayerState.SWIM);
        }
    }
    private void OnStealth(CallbackContext  input)
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

    public void SetPlayerState(PlayerState state)
    {
        this.state = state;
        OnStateChanged.Invoke(state);

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
        SetPlayerState(PlayerState.SWIM);
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.SWIM:
            {
                break;
            }
            case PlayerState.STEALTH:
            {
                _pounce.ChargeUpPounce();
                break;
            }
            case PlayerState.GRAB:
            {
                break;
            }
            case PlayerState.STUN:
                {
                    Stunned();
                    break;
                }
        } 
    }
}
