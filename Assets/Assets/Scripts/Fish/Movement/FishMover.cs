using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FishMover : MonoBehaviour
    {
        [Header("Target (optional)")]
        [SerializeField] private Transform target;

        [Header("Propulsion")]
        [SerializeField] private float maxSpeed = 2f;

        [Header("Steering")]
        [SerializeField] private float maxTurnRate = 100f;
        [SerializeField] private float turnResponsiveness = 2.5f;
        [SerializeField] private float turnAcceleration = 240f;

        [Header("Organic Movement")]
        [SerializeField] private float wanderAngle = 12f;
        [SerializeField] private float wanderFrequency = 0.2f;
        [SerializeField] private float lateralSlipDamping = 1.5f;
        
        private Vector2 _desiredDir;
        public Vector2 DesiredDirection
        {
            get => _desiredDir;
            set => _desiredDir = value.normalized;
        }

        private float _speed;
        
        private float _noiseSeed;
        private Rigidbody2D _rb;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _desiredDir = transform.right;
            _noiseSeed = Random.Range(0f, 1000f);
        }

        private void FixedUpdate()
        {
            if (_speed == 0)
            {
                _rb.linearVelocity = new Vector2(0,0);
                return;
            }
            Debug.DrawRay(transform.position, _desiredDir.normalized * (_speed / maxSpeed), Color.red);
            
            if (target)
            {
                Vector2 toTarget = (Vector2)target.position - _rb.position;
                if (toTarget.sqrMagnitude > 0.1f)
                    _desiredDir = toTarget.normalized;
            }
            
            float noise = Mathf.PerlinNoise(_noiseSeed, Time.fixedTime * wanderFrequency);
            float wanderOffset = (noise * 2f - 1f) * wanderAngle;
            
            float desiredAngle = Mathf.Atan2(_desiredDir.y, _desiredDir.x ) * Mathf.Rad2Deg;
            desiredAngle += wanderOffset;
            
            float angleError = Mathf.DeltaAngle(_rb.rotation, desiredAngle);
            float desiredAngularVelocity = Mathf.Clamp(angleError * turnResponsiveness, -maxTurnRate, maxTurnRate);
            _rb.angularVelocity = Mathf.MoveTowards(_rb.angularVelocity,
                desiredAngularVelocity,
                turnAcceleration * Time.fixedDeltaTime
            );
            
            /*float angularVelocityError = desiredAngularVelocity - _rb.angularVelocity;
            float torque = _rb.inertia * angularVelocityError * Mathf.Deg2Rad;
            _rb.AddTorque(torque);*/
            
            _rb.AddRelativeForce(Vector2.right * _speed);
            
            float sidewaysSpeed = Vector2.Dot(_rb.linearVelocity, transform.up);
            _rb.AddForce(-(Vector2)transform.up * (sidewaysSpeed * lateralSlipDamping));
            
            /*float slowTurnFactor = Mathf.InverseLerp(slowTurnAngle, 180f, Mathf.Abs(angleError));
            float minTurnSpeed = maxSpeed * 0.1f;
            float targetSpeed = Mathf.Lerp(maxSpeed, minTurnSpeed, slowTurnFactor);
            
            Vector2 desiredVelocity = (Vector2)transform.right * targetSpeed;
            _rb.linearVelocity = Vector2.MoveTowards(_rb.linearVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);*/
        }

        public void SetSpeed(FishSpeed speed)
        {
            _speed = speed switch
            {
                FishSpeed.Still => 0,
                FishSpeed.Chill => maxSpeed / 10,
                FishSpeed.Slow => maxSpeed / 6,
                FishSpeed.Normal => maxSpeed / 4,
                FishSpeed.Fast => maxSpeed / 2,
                FishSpeed.Max => maxSpeed,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }
    }
}