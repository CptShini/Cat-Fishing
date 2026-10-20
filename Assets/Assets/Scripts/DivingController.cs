using UnityEngine;


using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

using UnityEngine.Events;
using System;

public enum PlayerState
{
    SWIM, // Default state
    STEALTH,
    DASH,
}

public class DivingController : MonoBehaviour
{

    public float speed = 10, acceleration = 5, friction = 5;

    private float curSpeed = 0;


    [SerializeField] private AnimationCurve accelerationCurve;
    private Rigidbody2D playerBody;

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
        
        Debug.Log(newVelocity.ToString());

        float speedRatio = Mathf.Clamp01(playerBody.linearVelocity.magnitude / speed);
        float accelerationMultiplier = accelerationCurve.Evaluate(speedRatio);

        playerBody.linearVelocity = new Vector3(newVelocity.x, newVelocity.y, 0);
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
                
                break;
            }
            case PlayerState.DASH:
            {
                
                break;
            }
            
            default:
            {
                break;
            }
        } 
    }
}
