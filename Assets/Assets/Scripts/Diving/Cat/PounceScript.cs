using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Diving.Cat
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DamagedScript))]
    public class PounceScript : MonoBehaviour
    {
        [SerializeField] private StatBar staminaBar;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private Vector3 effectOffset;
        [SerializeField] private GameObject effect;
        
        public float curPounceStamina; // currrent player stamina, and current stamina for pounce
    
        private float pounceDuration; // Tracker for when to stop pounceing. Can change based on 
        private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
    
        private Camera _mainCamera;
        private Rigidbody2D _rb;

        public float curPounceCooldown;

        [SerializeField] private float _staminaRegenDuration = 1.5f;
        private float _curStaminaRegenCooldown = 0;

        private bool isStealthPlaying;

        public readonly UnityEvent<Vector2> OnPouncing = new();

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
            staminaBar.SetMaxValue(DivingPlayerStats.Instance.maxStamina);
            staminaBar.SetMinValue(0);
            
            DamagedScript damaged = GetComponent<DamagedScript>();
            damaged.OnDamaged.AddListener(CancelPounce);
        }

        public void ChargeUpPounce()
        {
            float maxPounceStamina =
                Mathf.Min(staminaBar.CurrentValue, DivingPlayerStats.Instance.maxPounceStamina);      

            float pounceChargeStamina = DivingPlayerStats.Instance.chargeUpSpeed * Time.deltaTime;
            curPounceStamina = Mathf.Clamp(curPounceStamina + pounceChargeStamina, 0,maxPounceStamina);

            if (isStealthPlaying) return;
            
            SFXManager.Instance.Play(SFXId.Stealth);
            isStealthPlaying = true;
        }
    
        public void StartPounce()
        {
            if (!transform.TryGetMouseWorldPosition(_mainCamera, out Vector3 mouseWorldPosition))
                return;

            curPounceStamina = Mathf.Max(0, curPounceStamina);
            pounceDirection = (Vector2)mouseWorldPosition - (Vector2)transform.position;
            pounceDuration = curPounceStamina / 20f;

            DivingController.Instance.SetPlayerState(PlayerState.PlayerState.POUNCE);
            staminaBar.CurrentValue -= curPounceStamina;
            Debug.Log(curPounceStamina);

            SFXManager.Instance.Play(SFXId.Pounce);
            isStealthPlaying = false;
            GameObject effectInstance = Instantiate(effect);

            Vector3 bodyPosition = _body.transform.position;
            Vector3 effectPosition = bodyPosition + effectOffset;
            if (_body.flipY)
            {
                effectPosition = new(
                    bodyPosition.x + effectOffset.x,
                    bodyPosition.y - effectOffset.y,
                    bodyPosition.z
                );
            }

            effectInstance.transform.position = effectPosition;
            effectInstance.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            effectInstance.transform.RotateAround(bodyPosition, Vector3.forward, _body.transform.rotation.eulerAngles.z);
        }

        public void Pounce()
        {
            Vector2 targetVelocity = pounceDirection.normalized * (DivingPlayerStats.Instance.pounceSpeed + curPounceStamina * 0.5f);
            Debug.Log($"POUNCE: {targetVelocity.magnitude}, POUNCE SPEED {DivingPlayerStats.Instance.pounceSpeed}, NORMAL SPEED: {DivingPlayerStats.Instance.speed}");
            _rb.linearVelocity = new Vector3(targetVelocity.x, targetVelocity.y, 0); // instantaneous max speed

            OnPouncing.Invoke(_rb.linearVelocity);   
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
                        curPounceStamina = 0;
                        _curStaminaRegenCooldown = 0;
                        curPounceCooldown = DivingPlayerStats.Instance.pounceCooldown;
                        DivingController.Instance.SetPlayerState(PlayerState.PlayerState.SWIM);
                    }
                    break;
                }
                case PlayerState.PlayerState.SWIM:
                {
                    if(_curStaminaRegenCooldown < _staminaRegenDuration)
                    {
                        _curStaminaRegenCooldown += Time.deltaTime;
                        return;
                    }
                    float staminaRegen = DivingPlayerStats.Instance.staminaRegen * Time.deltaTime;
                    staminaBar.CurrentValue += staminaRegen;
                    break;
                }
            }
        }
    }
}
