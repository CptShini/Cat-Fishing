using Assets.Scripts.PlayerState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PounceScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private DivingPlayerStats stats;
    public float curPounceStamina = 0; // currrent player stamina, and current stamina for pounce
    private float pounceDuration = 0; // Tracker for when to stop pounceing. Can change based on 
    private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D playerBody;

    public float curPounceCooldown;

    void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        stats = DivingPlayerStats.Instance;
    }

    public void ChargeUpPounce()
    {
        curPounceStamina = Mathf.Clamp(
            curPounceStamina + Time.deltaTime * stats.chargeUpSpeed, 
            0,
            StaminaBar.Instance.currentValue < stats.maxPounceStamina ? StaminaBar.Instance.currentValue : stats.maxPounceStamina);
    }
    public void StartPounce()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        SetPounceDirection(new Vector2(transform.position.x, transform.position.y), mousePos);

        pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now
        
        DivingController.Instance.SetPlayerState(PlayerState.POUNCE);
        StaminaBar.Instance.SetCurrentValue(Mathf.Clamp(StaminaBar.Instance.currentValue - curPounceStamina, 5, stats.maxStamina));
    }

    public void SetPounceDirection(Vector2 src, Vector2 dst)
    {
        pounceDirection = dst - src;
    }

    public void Pounce()
    {
        Vector2 targetVelocity = pounceDirection.normalized * (stats.pounceSpeed + curPounceStamina * 0.5f);

        playerBody.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed
    }

        // Update is called once per frame
    void Update()
    {
        if (DivingController.Instance.state != PlayerState.GRAB) // You cannot lose oxygen while battling a fish
        {
            curPounceCooldown -= Time.deltaTime;
        }
        
        if(DivingController.Instance.state == PlayerState.POUNCE)
        {
            Pounce();
            pounceDuration -= Time.deltaTime;

            if(pounceDuration < 0)
                {
                    curPounceCooldown = stats.pounceCooldown; // start cooldown
                    DivingController.Instance.SetPlayerState(PlayerState.SWIM);
                }
        }
    }
}
