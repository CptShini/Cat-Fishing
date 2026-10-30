using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Fish.Navigation
{
    [RequireComponent(typeof(FishEyes))]
    public class FishNavigator : MonoBehaviour
    {
        public Vector2 DesiredDirection { get; private set; }
        public Vector2 CurrentDirection { get; private set; }
        
        private FishEyes _eyes;
        
        [SerializeField] private Transform target;
        [SerializeField] private float tolerance;
        [SerializeField] private string[] areaMask;

        private Vector2[] corners;
        
        private void Update()
        {
            int mask = areaMask
                .Select(NavMesh.GetAreaFromName)
                .Where(mi => mi != -1)
                .Aggregate(0, (current, mi) => current | 1 << mi);

            NavQuery.TryGetPath(transform.position, target.position, out corners, mask, tolerance);
        }

        private void OnDrawGizmos()
        {
            if (corners == null || corners.Length == 0)
                return;
            
            Vector2 prev = transform.position;
            foreach (Vector2 corner in corners)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(corner, tolerance);
                Gizmos.DrawLine(prev, corner);
                
                prev = corner;
            }
        }
    }
}