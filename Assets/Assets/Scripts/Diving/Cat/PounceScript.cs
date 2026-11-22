using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Audio.SFX;
using Assets.Scripts.Diving.Bars;
using Assets.Scripts.Diving.Hazards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Diving.Cat
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DamagedScript))]
    public class PounceScript : MonoBehaviour, IEnvironmentAffectedMovement
    {
        [SerializeField] private StatBar staminaBar;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private Vector3 effectOffset;
        [SerializeField] private GameObject effect;
        
        public float curPounceStamina; // currrent player stamina, and current stamina for pounce
    
        private float pounceDuration; // Tracker for when to stop pounceing. Can change based on 
        private Vector2 pounceDirection; // For saving the pounce direction during multiple updates
        private readonly List<EnvironmentalMovementAffector> affectors = new();
        private Vector2 curEnvVelociy = new();
        private float acceleration;
        private float friction;
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
            acceleration = DivingPlayerStats.Instance.acceleration;
            friction = DivingPlayerStats.Instance.friction;
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
            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            curEnvVelociy = Vector2.zero;
            curPounceStamina = Math.Max(0, curPounceStamina);
            pounceDirection = mousePos - (Vector2)transform.position;
            pounceDuration = curPounceStamina / 20; // 5 stamina = 1 second, for now
        
            DivingController.Instance.SetPlayerState(PlayerState.PlayerState.POUNCE);
            staminaBar.CurrentValue -= curPounceStamina;


            SFXManager.Instance.Play(SFXId.Pounce);
            isStealthPlaying = false;
            GameObject effectInstance = Instantiate(effect);

            if(_body.flipY)
            {
                effectInstance.transform.position = new Vector3(_body.transform.position.x + effectOffset.x, _body.transform.position.y - effectOffset.y, _body.transform.position.z);
            }
            else
            {
                effectInstance.transform.position = _body.transform.position + effectOffset;
            }
            effectInstance.transform.rotation = Quaternion.Euler(0,0,90); 
            effectInstance.transform.RotateAround(_body.transform.position, new Vector3(0,0,1), _body.transform.rotation.eulerAngles.z); 
        }

        public void Pounce()
        {
            Vector2 targetVelocity = pounceDirection.normalized * (DivingPlayerStats.Instance.pounceSpeed + curPounceStamina * 0.5f);
            
            Vector2 environmentVelocity = CalculateVelocityChange();
            float curAcceleration = environmentVelocity == Vector2.zero ? friction : acceleration;

            curEnvVelociy = Vector2.MoveTowards(
                curEnvVelociy,
                environmentVelocity,
                curAcceleration * Time.deltaTime
            ); 

            _rb.linearVelocity = targetVelocity + curEnvVelociy; // instantaneous max speed

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

        public void AddAffector(EnvironmentalMovementAffector affector)
        {
            affectors.Add(affector);
        }

        public void RemoveAffector(EnvironmentalMovementAffector affector)
        {
            affectors.Remove(affector);
        }

        public Vector2 CalculateVelocityChange()
        {
            Vector2 velocity = new();
            return affectors.Aggregate(velocity, (current, affector) => current + affector.Velocity);
        }

    }
}
