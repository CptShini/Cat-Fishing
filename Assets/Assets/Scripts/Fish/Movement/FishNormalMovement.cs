using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Movement
{
    [RequireComponent(typeof(FishMover))]
    public class FishNormalMovement : MonoBehaviour
    {
        [SerializeField] private float viewDistance = 3f;
        [SerializeField] private float avoidanceFactor = 10f;

        [SerializeField] private Transform eyes;

        private FishMover _mover;

        private void Start() => _mover = GetComponent<FishMover>();
        
        private void Update()
        {
            //Debug.DrawRay(transform.position, _desiredDir.normalized * (_speed / maxSpeed), Color.red);
            See();
        }

        public void Wander(FishSpeed speed)
        {
            _mover.SetDesiredDirection(Random.onUnitCircle);
            //SetSpeed(speed);
        }

        public void Flee(FishSpeed speed, Vector2 fleePoint)
        {
            _mover.SetDesiredDirection((Vector2)transform.position - fleePoint);
            //SetSpeed(speed);
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
            //_desiredDir = Vector3.Slerp(_desiredDir.normalized, awayDir.normalized, weight * Time.deltaTime).normalized;
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