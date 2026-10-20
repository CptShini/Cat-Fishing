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

    public float speed = 10, stealthSpeed = 3, pounceSpeed = 20, acceleration = 5, friction = 5;

    private float curSpeed = 0;
    private float pounceDuration = 0; // Tracker for when to stop pounceing. Can change based on 
    private Vector2 pounceDirection; // For saving the pounce direction during multiple updates

    //[SerializeField] private AnimationCurve accelerationCurve;
    private Rigidbody2D playerBody;

    [SerializeField] private Camera mainCamera;

    public Vector2 velocityVector;

    public PlayerState state;

    void Awake()
    {
        state = PlayerState.SWIM;
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

    public void Swim()
    {
        Vector2 targetVelocity = velocityVector.normalized * speed;

        float currentAcceleration = (velocityVector.x != 0) ? acceleration : friction;

        Vector2 newVelocity = Vector2.MoveTowards(
            playerBody.linearVelocity,
            targetVelocity,
            currentAcceleration * Time.fixedDeltaTime
        );
        
        //Debug.Log(newVelocity.ToString());

        float speedRatio = Mathf.Clamp01(playerBody.linearVelocity.magnitude / speed);

        playerBody.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);
    }

    public void Stealth()
    {
        Vector2 targetVelocity = velocityVector.normalized * stealthSpeed;

        float currentAcceleration = (velocityVector.x != 0) ? acceleration : friction;

        Vector2 newVelocity = Vector2.MoveTowards(
            playerBody.linearVelocity,
            targetVelocity,
            currentAcceleration * Time.fixedDeltaTime
        );
        
        //Debug.Log(newVelocity.ToString());

        float speedRatio = Mathf.Clamp01(playerBody.linearVelocity.magnitude / stealthSpeed);

        playerBody.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);
    }

    public void Pounce(Vector2 direction)
    {

        Vector2 targetVelocity = direction.normalized * pounceSpeed;

        playerBody.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed


    }

    public void OnStealth(CallbackContext  input)
    {
        if (input.performed) // Did the player press?
        {
            if(state == PlayerState.SWIM)
            {
                state = PlayerState.STEALTH;
            }
            Debug.Log("START STEALTH");
        }
        else if (input.canceled) // Did the player release
        {

            if(state == PlayerState.STEALTH)
            {
                Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

                SetPounceDirection(new Vector2(transform.position.x, transform.position.y), mousePos);

                pounceDuration = 5;

                state = PlayerState.POUNCE;
            }
            Debug.Log("RELEASE STEALTH");
        }

    }

    public void SetPounceDirection(Vector2 src, Vector2 dst)
    {
        
        pounceDirection = dst - src;
        Debug.Log("SRC: " + src.ToString());
        Debug.Log("DST: " + dst.ToString());

        Debug.Log("NEW POUNCE DIRECTION: " + pounceDirection.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.SWIM:
            {
                Swim();
                break;
            }
            case PlayerState.STEALTH:
            {
                Stealth();
                break;
            }
            case PlayerState.POUNCE:
            {

                Pounce(pounceDirection);
                pounceDuration -= Time.fixedDeltaTime;

                if(pounceDuration < 0)
                    {
                        Debug.Log("STOP POUNCING");
                        state = PlayerState.SWIM;
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
