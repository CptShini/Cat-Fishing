using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FishNormalMovement : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 3f, turnSpeed = 1f;
        [SerializeField] private float viewDistance = 3f;
        [SerializeField] private float avoidanceFactor = 10f;

        [SerializeField] private Transform eyes;

        private Vector2 _desiredDir;
        private float _speed;
    
        private Rigidbody2D _rb;
    
        private void Start() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            Debug.DrawRay(transform.position, _desiredDir.normalized * (_speed / maxSpeed), Color.red);
            See();
        
            _rb.AddForce(_desiredDir.normalized * (_speed * 100f * Time.deltaTime), ForceMode2D.Force);
            float angle = Vector2.SignedAngle(transform.right, _desiredDir);
            _rb.AddTorque(angle * turnSpeed * Time.deltaTime, ForceMode2D.Force);
        }

        public void Wander(FishSpeed speed)
        {
            _desiredDir = Random.onUnitCircle;
            SetSpeed(speed);
        }

        public void Flee(FishSpeed speed, Vector2 fleePoint)
        {
            _desiredDir = (Vector2)transform.position - fleePoint;
            SetSpeed(speed);
        }

        private void SetSpeed(FishSpeed speed)
        {
            _speed = speed switch
            {
                FishSpeed.Chill => maxSpeed / 6,
                FishSpeed.Normal => maxSpeed / 4,
                FishSpeed.Fast => maxSpeed / 2,
                FishSpeed.Max => maxSpeed,
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }

        private void See()
        {
            List<RaycastHit2D> hits = new();
            for (int i = -60; i <= 60; i += 15)
            {
                RaycastHit2D hit = Raycast(i);
                hits.Add(hit);
            }
        
            bool hitAny = hits.Any(hit => hit.collider);
            if (!hitAny) return;
        
            Vector2 avgPoint = GetAverageHitPoint(hits);
            Vector2 awayDir = (Vector2)eyes.position - avgPoint;
            
            Debug.DrawLine(eyes.position, avgPoint, Color.blue);
            Debug.DrawRay(transform.position, awayDir, Color.green);
        
            float weight = (viewDistance - awayDir.magnitude) * avoidanceFactor;
            _desiredDir = Vector3.Slerp(_desiredDir.normalized, awayDir.normalized, weight * Time.deltaTime).normalized;
        }
    
        private RaycastHit2D Raycast(int angleOffset)
        {
            Vector2 source = eyes.position;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * transform.right;
            Debug.DrawRay(source, dir * viewDistance, Color.red);
            return Physics2D.Raycast(source, dir, viewDistance);
        }

        private Vector2 GetAverageHitPoint(IEnumerable<RaycastHit2D> hits)
        {
            float x = 0f, y = 0f, weightSum = 0f;
            foreach (RaycastHit2D hit in hits.Where(hit => hit.collider))
            {
                Debug.DrawLine(eyes.transform.position, hit.point, Color.white);
                float weight = (viewDistance - hit.distance) / viewDistance;
                x += hit.point.x * weight;
                y += hit.point.y * weight;
                weightSum += weight;
            }
        
            return new(x / weightSum, y / weightSum);
        }
    }
}