using Assets.Scripts.Diving.Bars;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Diving.Cat
{
    public class PounceScript : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private StatBar staminaBar;
    
        private DamagedScript damaged;
        public float curPounceStamina; // currrent player stamina, and current stamina for pounce
    
        private float pounceDuration; // Tracker for when to stop pounceing. Can change based on 
        private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
    
        private Rigidbody2D _rb;

        public float curPounceCooldown;

        private bool isStealthPlaying;

        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            staminaBar.SetMaxValue(DivingPlayerStats.Instance.maxStamina);
            staminaBar.SetMinValue(5); // Idk, ask Andreas
            damaged = gameObject.GetComponent<DamagedScript>();
            damaged.OnDamaged.AddListener(CancelPounce);
        }

        public void ChargeUpPounce()
        {
            float maxPounceStamina =
                Mathf.Min(staminaBar.CurrentValue, DivingPlayerStats.Instance.maxPounceStamina);
        
            float pounceChargeStamina = DivingPlayerStats.Instance.chargeUpSpeed * Time.deltaTime;
            curPounceStamina = Mathf.Clamp(curPounceStamina + pounceChargeStamina, 0,maxPounceStamina);

        if (!isStealthPlaying)
        {
            AudioManager.Instance.PlayStealth();
            isStealthPlaying = true;
        }
        }
    
        public void StartPounce()
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            pounceDirection = mousePos - (Vector2)transform.position;
            pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now
        
            DivingController.Instance.SetPlayerState(PlayerState.PlayerState.POUNCE);
            staminaBar.CurrentValue -= curPounceStamina;

            AudioManager.Instance.PlayPounce();
            isStealthPlaying = false;
        }

        public void Pounce()
        {
            Vector2 targetVelocity = pounceDirection.normalized * (DivingPlayerStats.Instance.pounceSpeed + curPounceStamina * 0.5f);

            _rb.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed
        }

        private void CancelPounce()
        {
            pounceDuration = float.MinValue;
        }

        // Update is called once per frame
        private void Update()
        {
            var playerState = DivingController.Instance.state;
        
            if (playerState != PlayerState.PlayerState.GRAB)
            {
                curPounceCooldown -= Time.deltaTime;
            }

            switch (playerState)
            {
                case PlayerState.PlayerState.POUNCE:
                {
                    if (pounceDuration >= 0){
                        Pounce();
                        pounceDuration -= Time.deltaTime;
                    }
                    else{
                        curPounceCooldown = DivingPlayerStats.Instance.pounceCooldown;
                        DivingController.Instance.SetPlayerState(PlayerState.PlayerState.SWIM);
                    }
                    break;
                }
                case PlayerState.PlayerState.SWIM:
                {
                    float staminaRegen = DivingPlayerStats.Instance.staminaRegen * Time.deltaTime;
                    staminaBar.CurrentValue += staminaRegen;
                    break;
                }
            }
        }
    }
}
