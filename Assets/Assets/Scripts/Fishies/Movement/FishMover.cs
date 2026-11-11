using System;
using Assets.Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fishies.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FishMover : MonoBehaviour
    {
        [Header("Steering")]
        [SerializeField] private float minTurnRate = 90f;
        [SerializeField] private float maxTurnRate = 360f;
        [SerializeField] private float turnSmoothTime = 0.4f;

        private float _turnVelocity;
        private float _wanderTime;

        [Header("Movement")]
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float turnBraking = 6f;
        [SerializeField] private float slowTurnAngle = 65f;
        [SerializeField, Range(0f, 1f)]
        private float minSpeedWhileTurning = 0.6f;

        [SerializeField] private float lateralSlipDamping = 0.7f;

        [Header("Speed Stat Conversion")]
        [SerializeField] private float speedStatScale = 1f;

        [Header("Organic Movement")]
        [SerializeField] private float wanderAngle = 22f;
        [SerializeField] private float wanderFrequency = 0.35f;
        
        [Header("Destination")]
        [SerializeField] private float slowDownDistance = 5f;
        [SerializeField] private float arrivalDeceleration = 12f;

        public Vector2 Heading;
        public float NearDistance;

        private float _maxSpeed;
        private float _speed;

        private float _noiseSeed;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            Heading = transform.right;
            _noiseSeed = Random.Range(0f, 1000f);
            Init(3);
            SetSpeed(FishSpeed.Normal);
        }

        public void Init(int speedStat)
        {
            _maxSpeed = speedStat * speedStatScale;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            // -----------------------------------------------
            // Speed-dependent wandering
            // -----------------------------------------------

            float speed01 = _maxSpeed > 0.001f
                ? Mathf.Clamp01(_rb.linearVelocity.magnitude / _maxSpeed)
                : 0f;
            
            float currentWanderFrequency = wanderFrequency *
                Mathf.Lerp(0.6f, 1.8f, speed01);

            _wanderTime += dt * currentWanderFrequency;
            
            float noise = Mathf.PerlinNoise(_noiseSeed, _wanderTime);
            float currentWanderAngle = wanderAngle * Mathf.Lerp(0.8f, 1.3f, speed01);
            float wanderOffset = (noise * 2f - 1f) * currentWanderAngle;

            // -----------------------------------------------
            // Steering
            // -----------------------------------------------

            float currentAngle = Mathf.Atan2(Heading.y, Heading.x) * Mathf.Rad2Deg;
            float desiredAngle = currentAngle + wanderOffset;
            
            float angleError = Mathf.DeltaAngle(_rb.rotation, desiredAngle);
            
            float turnSpeed01 = _maxSpeed > 0.001f
                ? Mathf.Clamp01(_speed / _maxSpeed)
                : 0f;

            float currentMaxTurnRate = Mathf.Lerp(
                minTurnRate,
                maxTurnRate,
                turnSpeed01
            );

            float nextRotation = Mathf.SmoothDampAngle(
                _rb.rotation,
                desiredAngle,
                ref _turnVelocity,
                turnSmoothTime,
                currentMaxTurnRate,
                dt
            );

            _rb.MoveRotation(nextRotation);

            // -----------------------------------------------
            // Movement directions
            // -----------------------------------------------

            float radians = nextRotation * Mathf.Deg2Rad;
            Vector2 forward = new(Mathf.Cos(radians), Mathf.Sin(radians));
            Vector2 sideways = new(-forward.y, forward.x);

            // -----------------------------------------------
            // Turning slowdown
            // -----------------------------------------------

            float turnAmount = Mathf.InverseLerp(
                slowTurnAngle,
                180f,
                Mathf.Abs(angleError)
            );
            
            float arrivalFactor = slowDownDistance > 0f
                ? Mathf.SmoothStep(
                    0f,
                    1f,
                    Mathf.Clamp01(NearDistance / slowDownDistance)
                )
                : 1f;

            float targetSpeed = _speed
                                * Mathf.Lerp(1f, minSpeedWhileTurning, turnAmount)
                                * arrivalFactor;

            float currentAcceleration = Mathf.Lerp(
                acceleration,
                turnBraking,
                turnAmount
            );

            // -----------------------------------------------
            // Velocity
            // -----------------------------------------------

            Vector2 velocity = _rb.linearVelocity;
            float forwardSpeed = Vector2.Dot(velocity, forward);
            float newForwardSpeed = Mathf.MoveTowards(
                forwardSpeed,
                targetSpeed,
                currentAcceleration * dt
            );

            velocity += forward * (newForwardSpeed - forwardSpeed);
            
            float sidewaysSpeed = Vector2.Dot(velocity, sideways);
            velocity -= sideways * (sidewaysSpeed * (1f - Mathf.Exp(-lateralSlipDamping * dt)));
            velocity = Vector2.ClampMagnitude(velocity, _maxSpeed);

            _rb.linearVelocity = velocity;
        }

        public void SetSpeed(FishSpeed speed)
        {
            float speedMultiplier = speed switch
            {
                FishSpeed.Chill => 0.1f,
                FishSpeed.Slow => 1f / 6f,
                FishSpeed.Normal => 0.25f,
                FishSpeed.Fast => 0.5f,
                FishSpeed.Max => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };

            _speed = _maxSpeed * speedMultiplier;
        }

        private void OnDisable()
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        private void OnDrawGizmos()
        {
            if (!DebugDrawer.Instance
                || !DebugDrawer.Instance.EnableDebug
                || !DebugDrawer.Instance.Fish_Mover
            ) return;

            if (DebugDrawer.Instance.Fish_Mover_DrawHeading) DrawHeading();
            
            return;

            void DrawHeading()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, Heading);
                
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _rb.linearVelocity);
            }
        }
    }
}