using Assets.Scripts.Diving.Bars;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.InputSystem;

public class PounceScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    public float curPounceStamina; // currrent player stamina, and current stamina for pounce
    
    private float pounceDuration; // Tracker for when to stop pounceing. Can change based on 
    private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
    
    private Rigidbody2D playerBody;

    public float curPounceCooldown;

    private void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void ChargeUpPounce()
    {
        float maxPounceStamina =
            Mathf.Min(StaminaBar.Instance.CurrentValue, DivingPlayerStats.Instance.maxPounceStamina);
        
        float pounceChargeStamina = DivingPlayerStats.Instance.chargeUpSpeed * Time.deltaTime;
        curPounceStamina = Mathf.Clamp(curPounceStamina + pounceChargeStamina, 0,maxPounceStamina);
    }
    
    public void StartPounce()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        pounceDirection = mousePos - (Vector2)transform.position;
        pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now
        
        DivingController.Instance.SetPlayerState(PlayerState.POUNCE);
        StaminaBar.Instance.CurrentValue -= curPounceStamina;
    }

    public void Pounce()
    {
        Vector2 targetVelocity = pounceDirection.normalized * (DivingPlayerStats.Instance.pounceSpeed + curPounceStamina * 0.5f);

        playerBody.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed
    }

        // Update is called once per frame
    private void Update()
    {
        if (DivingController.Instance.state != PlayerState.GRAB) // You cannot lose oxygen while battling a fish
        {
            curPounceCooldown -= Time.deltaTime;
        }

        if (DivingController.Instance.state != PlayerState.POUNCE) return;
        
        Pounce();
        pounceDuration -= Time.deltaTime;

        if (pounceDuration >= 0) return;
            
        curPounceCooldown = DivingPlayerStats.Instance.pounceCooldown; // start cooldown
        DivingController.Instance.SetPlayerState(PlayerState.SWIM);
    }
}
