using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fishies.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FishMover : MonoBehaviour
    {
        [Header("Steering")]
        [SerializeField] private float maxTurnRate = 100f;
        [SerializeField] private float turnResponsiveness = 2.5f;
        [SerializeField] private float turnAcceleration = 240f;

        [Header("Organic Movement")]
        [SerializeField] private float wanderAngle = 12f;
        [SerializeField] private float wanderFrequency = 0.2f;
        [SerializeField] private float lateralSlipDamping = 1.5f;
        
        public Vector2 DesiredDirection;

        private float _maxSpeed;
        private float _speed;
        
        private float _noiseSeed;
        private Rigidbody2D _rb;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            DesiredDirection = transform.right;
            _noiseSeed = Random.Range(0f, 1000f);
        }

        public void Init(int speedStat)
        {
            _maxSpeed = speedStat; // TODO: proper conversion formula
        }

        private void FixedUpdate()
        {
            float noise = Mathf.PerlinNoise(_noiseSeed, Time.fixedTime * wanderFrequency);
            float wanderOffset = (noise * 2f - 1f) * wanderAngle;
            
            float desiredAngle = Mathf.Atan2(DesiredDirection.y, DesiredDirection.x ) * Mathf.Rad2Deg;
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
                FishSpeed.Chill => _maxSpeed / 10,
                FishSpeed.Slow => _maxSpeed / 6,
                FishSpeed.Normal => _maxSpeed / 4,
                FishSpeed.Fast => _maxSpeed / 2,
                FishSpeed.Max => _maxSpeed,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }

        private void OnDisable()
        {
            _rb.linearVelocity = new(0,0);
            _rb.angularVelocity = 0f;
        }
    }
}