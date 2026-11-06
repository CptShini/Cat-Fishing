using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fishies.Navigation
{
    public class FishEyes : MonoBehaviour
    {
        public bool Spotted { get; private set; }
        public float Weight { get; private set; }
        public Vector2 AwayDirection { get; private set; }
        
        [SerializeField] private Transform eyes;
        [SerializeField] private float viewDistance = 1.5f;

        [Space]
        
        [SerializeField] private float fov = 120f;
        [SerializeField] private int eyeCount = 4;

        private void Update() => See();

        private void See()
        {
            if (eyeCount <= 0 || fov / eyeCount <= 0) return;
            
            List<RaycastHit2D> hits = new();
            for (float i = -fov / 2f; i <= fov / 2f; i += fov / eyeCount)
            {
                RaycastHit2D hit = Raycast(i);
                hits.Add(hit);
            }
        
            Spotted = hits.Any(hit => hit.collider);
            if (!Spotted) return;
        
            Vector2 closestPoint = GetClosestHit(hits);
            Vector2 awayDir = (Vector2)eyes.position - closestPoint;
        
            Weight = (viewDistance - awayDir.magnitude) / viewDistance;
            AwayDirection = awayDir.normalized;

            if (!DebugDrawer.Instance 
                || !DebugDrawer.Instance.Fish_Eyes
                || !DebugDrawer.Instance.Fish_Eyes_DrawDirections
            ) return;
            
            Debug.DrawRay(eyes.position, AwayDirection * (Weight * 3), Color.red);
        }
    
        private RaycastHit2D Raycast(float angleOffset)
        {
            Vector2 source = eyes.position;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * transform.right;
            
            if (DebugDrawer.Instance 
                && DebugDrawer.Instance.Fish_Eyes 
                && DebugDrawer.Instance.Fish_Eyes_DrawRaycasts
            ) Debug.DrawRay(source, dir * viewDistance, Color.red);
                
            return Physics2D.Raycast(source, dir, viewDistance);
        }

        private Vector2 GetClosestHit(IEnumerable<RaycastHit2D> hits)
        {
            Vector2 closest = Vector2.zero;
            
            float min = float.MaxValue;
            foreach (RaycastHit2D hit in hits.Where(hit => hit.collider))
            {
                if (DebugDrawer.Instance
                    && DebugDrawer.Instance.Fish_Eyes
                    && DebugDrawer.Instance.Fish_Eyes_DrawRaycasts
                ) Debug.DrawLine(eyes.position, hit.point, Color.white);

                float dist = hit.distance;
                if (dist >= min) continue;
                
                min = dist;
                closest = hit.point;
            }
        
            return closest;
        }
    }
}