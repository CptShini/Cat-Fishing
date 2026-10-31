using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Navigation
{
    [RequireComponent(typeof(FishEyes))]
    public class FishNavigator : MonoBehaviour
    {
        public readonly UnityEvent DestinationReached = new();
        
        public Vector2 CurrentDirection { get; private set; }
        private Vector2 _desiredDirection;
        
        private bool _hasDestination;
        private readonly Queue<Vector2> _path = new();

        private bool _hasWaypoint;
        private Vector2 _waypoint;

        private int areaMask;
        
        private FishEyes _eyes;

        [SerializeField] private Transform eyePoint;
        [SerializeField] private string areaMaskName;
        [SerializeField] private float arrivalDistanceTolerance;
        [SerializeField] private float fleeDistMin, fleeDistMax;
        [SerializeField] private float wanderDistMin, wanderDistMax;

        private void Awake()
        {
            _eyes = GetComponent<FishEyes>();

            areaMask = NavMesh.GetAreaFromName(areaMaskName);
            if (areaMask != -1) areaMask = 1 << areaMask;
            
            CurrentDirection = Random.onUnitCircle;
        }
        
        private void Update()
        {
            Navigate();
            
            if (!_hasDestination) return;
            
            CurrentDirection = _eyes.Spotted
                ? Vector3.Slerp(_desiredDirection, _eyes.AwayDirection, _eyes.Weight)
                : _desiredDirection;
        }

        private void Navigate()
        {
            if (!_hasDestination) return;
            
            if (!_hasWaypoint)
            {
                _waypoint = _path.Peek();
                _hasWaypoint = true;
            }
            
            float distToWaypoint = Vector2.Distance(transform.position, _waypoint);
            bool arrivedAtWaypoint = distToWaypoint <= arrivalDistanceTolerance;
            if (!arrivedAtWaypoint)
            {
                _desiredDirection = (_waypoint - (Vector2)transform.position).normalized;
                return;
            }
            
            _path.Dequeue();
            _hasWaypoint = false;

            bool arrivedAtDestination = _path.Count == 0;
            if (!arrivedAtDestination) return;
            
            _hasDestination = false;
            _desiredDirection = Vector2.zero;
            DestinationReached.Invoke();
        }

        public void Wander()
        {
            Vector2[] path = Pathfind(0.5f, () =>
            {
                Vector2 newDirection = MixWithRandom(CurrentDirection);
                float wanderDistance = Random.Range(wanderDistMin, wanderDistMax);

                var hit = Physics2D.Raycast(eyePoint.position, newDirection, wanderDistance);
                Vector2 wanderTarget = hit.collider 
                    ? hit.point 
                    : (Vector2)transform.position + newDirection * wanderDistance;
                
                return wanderTarget;
            });
            
            SetPath(path);
        }
        
        public void Flee(Vector2 fleePoint)
        {
            Vector2[] path = Pathfind(fleeDistMax, () =>
            {
                Vector2 awayDir = ((Vector2)transform.position - fleePoint).normalized;
                Vector2 fleeDir = MixWithRandom(awayDir);
            
                float fleeDistance = Random.Range(fleeDistMin, fleeDistMax);
                Vector2 fleeTarget = (Vector2)transform.position + fleeDir * fleeDistance;
                return fleeTarget;
            });
            
            FilterFurthestPath();
            SetPath(path);
            
            return;
            
            void FilterFurthestPath()
            {
                float max = float.MinValue;
            
                int maxIndex = 0;
                for (int i = 0; i < path.Length; i++)
                {
                    float dist = SqrDist(path[i]);
                    if (dist <= max) continue;
                
                    max = dist;
                    maxIndex = i;
                }
            
                path = path.Take(maxIndex + 1).ToArray();
                return;
            
                float SqrDist(Vector2 p)
                {
                    float a = transform.position.x - p.x;
                    float b = transform.position.y - p.y;
                    return a * a + b * b;
                }
            }
        }

        private void SetPath(Vector2[] path)
        {
            if (path.Length == 0) return;
            
            _hasDestination = true;
            _hasWaypoint = false;
            
            _path.Clear();
            foreach (Vector2 p in path)
                _path.Enqueue(p);
        }

        private Vector2[] Pathfind(float tolerance, Func<Vector2> randomDestinationFunc)
        {
            const int maxIterations = 25;
            for (int i = 0; i < maxIterations; i++)
            {
                bool validPath = TryPathfind(randomDestinationFunc(), out Vector2[] path);
                if (validPath) return path;
            }
            
            Debug.LogWarning($"{name} didn't find any path!");
            return Array.Empty<Vector2>();
            
            bool TryPathfind(Vector2 destination, out Vector2[] path) =>
                NavQuery.TryGetPath(transform.position, destination, out path, areaMask, tolerance);
        }

        private static Vector2 MixWithRandom(Vector2 vec)
        {
            float r = Random.Range(0f, 1f); // Completely arbitrary
            return Vector3.Slerp(vec, Random.onUnitCircle, r*r);
        }

        private void OnDrawGizmos()
        {
            if (!DebugDrawer.Instance
                || !DebugDrawer.Instance.EnableDebug 
                || !DebugDrawer.Instance.Fish_Nav
            ) return;
            
            if (DebugDrawer.Instance.Fish_Nav_DrawPathing) DrawPath();
            if (DebugDrawer.Instance.Fish_Nav_DrawDirections) DrawDirections();

            return;

            void DrawDirections()
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _desiredDirection);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, CurrentDirection);
            }
            
            void DrawPath()
            {
                if (_path == null || _path.Count == 0)
                    return;
            
                Gizmos.color = Color.white;
                
                Vector2 prev = transform.position;
                foreach (Vector2 p in _path)
                {
                    Gizmos.DrawLine(prev, p);
                    Gizmos.DrawSphere(p, 0.1f);
                
                    prev = p;
                }
            }
        }
    }
}