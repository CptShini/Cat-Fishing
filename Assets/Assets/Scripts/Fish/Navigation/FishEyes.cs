using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Fish.Navigation
{
    public class FishEyes : MonoBehaviour
    {
        [SerializeField] private Transform eyes;
        [SerializeField] private float viewDistance = 3f;
        [SerializeField] private float avoidanceFactor = 10f;
        
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
            /*Vector2 newDirection = Vector3.Slerp(
                _mover.DesiredDirection.normalized,
                awayDir.normalized,
                weight
            );
            
            _mover.DesiredDirection = newDirection;*/
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