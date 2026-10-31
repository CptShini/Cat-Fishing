using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Navigation
{
    [RequireComponent(typeof(FishEyes))]
    public class FishNavigator : MonoBehaviour
    {
        public Vector2 DesiredDirection { get; private set; }
        public Vector2 CurrentDirection { get; private set; }
        
        private bool _hasDestination;
        private Vector2 _destination;
        private readonly Queue<Vector2> _path = new();

        private bool _hasWaypoint;
        private Vector2 _waypoint;

        private int areaMask;
        
        private FishEyes _eyes;
        
        [SerializeField] private Transform target;
        
        [Space]
        
        [SerializeField] private string areaMaskName;
        [SerializeField] private float arrivalDistanceTolerance;
        [SerializeField] private float fleeDistMin, fleeDistMax;

        [Space]
        
        [SerializeField] private float wanderDistMin, wanderDistMax;

        private void Start()
        {
            _eyes = GetComponent<FishEyes>();
            
            areaMask = 1 << NavMesh.GetAreaFromName(areaMaskName);
            //TryFlee(target.position);
            CurrentDirection = transform.right;
        }
        
        private void Update()
        {
            //DesiredDirection = (target.position - transform.position).normalized;
            if (!_hasDestination)
                Wander();
            Navigate();
            
            //compute CurrentDir from DesiredDir + WaverDir + AvoidanceDir
            if (_hasDestination) CurrentDirection = DesiredDirection;
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
                DesiredDirection = (_waypoint - (Vector2)transform.position).normalized;
                return;
            }
            
            _path.Dequeue();
            _hasWaypoint = false;

            bool arrivedAtDestination = _path.Count == 0;
            if (!arrivedAtDestination) return;
            
            _hasDestination = false;
            DesiredDirection = Vector2.zero;
        }

        public void Wander()
        {
            Vector2[] path = Pathfind(0.5f, () =>
            {
                Vector2 randomDirection = Random.onUnitCircle;

                float r = Random.Range(0f, 1f);
                Vector2 newDirection = Vector3.Slerp(CurrentDirection, randomDirection, r*r);
                
                float wanderDistance = Random.Range(wanderDistMin, wanderDistMax);
                Vector2 wanderTarget = (Vector2)transform.position + newDirection * wanderDistance;
                
                var hit = Physics2D.Raycast(transform.position, newDirection, wanderDistance);
                if (hit.collider)
                    wanderTarget = hit.point;
                
                Debug.DrawLine(transform.position, wanderTarget, Color.pink);
                Debug.DrawRay(transform.position, randomDirection, Color.red);
                Debug.DrawRay(transform.position, CurrentDirection, Color.green);
                Debug.DrawRay(transform.position, newDirection, Color.blue);
                
                return wanderTarget;
            });
            
            SetPath(path);
        }
        
        public void Flee(Vector2 fleePoint)
        {
            Vector2[] path = Pathfind(fleeDistMax, () =>
            {
                Vector2 awayDir = ((Vector2)transform.position - fleePoint).normalized;
            
                float fleeDistance = Random.Range(fleeDistMin, fleeDistMax);
                Vector2 fleeTarget = (Vector2)transform.position + awayDir * fleeDistance;
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
            _destination = path[^1];
            
            _path.Clear();
            foreach (Vector2 p in path)
                _path.Enqueue(p);
        }

        private Vector2[] Pathfind(float tolerance, Func<Vector2> randomDestinationFunc)
        {
            const int maxIterations = 100;
            for (int i = 0; i < maxIterations; i++)
            {
                bool validPath = TryPathfind(randomDestinationFunc(), out Vector2[] path);
                if (validPath) return path;
            }
            
            return Array.Empty<Vector2>();
            
            bool TryPathfind(Vector2 destination, out Vector2[] path) =>
                NavQuery.TryGetPath(transform.position, destination, out path, areaMask, tolerance);
        }

        private void OnDrawGizmos()
        {
            if (DebugDrawer.Instance == null || !DebugDrawer.Instance.EnableDebug) return;
            
            if (DebugDrawer.Instance.Fish_Nav_DrawPathing) DrawPath();
            if (DebugDrawer.Instance.Fish_Nav_DrawDirections) DrawDirections();

            return;

            void DrawDirections()
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, DesiredDirection);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, CurrentDirection);
            }
            
            void DrawPath()
            {
                if (_path == null || _path.Count == 0)
                    return;
            
                Gizmos.color = Color.red;
                
                Vector2 prev = transform.position;
                foreach (Vector2 p in _path)
                {
                    Gizmos.DrawLine(prev, p);
                    Gizmos.DrawSphere(p, 0.25f);
                
                    prev = p;
                }
            }
        }
    }
}