using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Range = Assets.Scripts.Helpers.Range;

namespace Assets.Scripts.Fishies.Navigation
{
    [RequireComponent(typeof(FishEyes))]
    public class FishNavigator : MonoBehaviour
    {
        public readonly UnityEvent DestinationReached = new();
        
        public Vector2 CurrentDirection { get; private set; }
        private Vector2 _desiredDirection;
        
        private bool _hasDestination;
        private Vector2 _destination;
        private readonly Queue<Vector2> _path = new();

        private bool _hasWaypoint;
        private Vector2 _waypoint;

        private int _areaMask;
        
        private FishEyes _eyes;

        [SerializeField] private Transform eyePoint;
        [SerializeField] private float arrivalDistanceTolerance;
        [SerializeField] private Range fleeDist;
        [SerializeField] private Range wanderDist;
        [SerializeField] private Range reNavTime;

        private void Awake()
        {
            _eyes = GetComponent<FishEyes>();
            CurrentDirection = Random.onUnitCircle;
        }

        public void Init(int biome) => _areaMask = biome;

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
            Vector2[] path = Pathfind(0.5f, urgency =>
            {
                Vector2 newDirection = MixWithRandom(CurrentDirection, urgency);
                float wanderDistance = wanderDist.Random();

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
            Vector2[] path = Pathfind(fleeDist.Max, urgency =>
            {
                Vector2 awayDir = ((Vector2)transform.position - fleePoint).normalized;
                Vector2 fleeDir = MixWithRandom(awayDir, urgency);
            
                float fleeDistance = fleeDist.Random();
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
            _destination = path[^1];
            
            _hasWaypoint = false;
            
            _path.Clear();
            foreach (Vector2 p in path)
                _path.Enqueue(p);
            
            StartCoroutine(RecalculatePath());
        }
        
        private IEnumerator RecalculatePath()
        {
            float waitTime = reNavTime.Random();
            yield return new WaitForSeconds(waitTime);
            
            if (!_hasDestination) yield break;
            
            TryPathfind(_destination, 0.5f, out Vector2[] path);
            SetPath(path);
        }

        private Vector2[] Pathfind(float tolerance, Func<float, Vector2> randomDestinationFunc)
        {
            const int maxIterations = 10;
            for (int i = 0; i < maxIterations; i++)
            {
                float urgency = (float)i / maxIterations;
                bool validPath = TryPathfind(
                    randomDestinationFunc(urgency),
                    tolerance + urgency * urgency * urgency * maxIterations,
                    out Vector2[] path
                );
                if (validPath) return path;
            }
            
            Debug.LogWarning($"{name} didn't find any path!");
            return Array.Empty<Vector2>();
        }
        
        private bool TryPathfind(Vector2 destination, float tolerance, out Vector2[] path) =>
            NavQuery.TryGetPath(transform.position, destination, out path, _areaMask, tolerance);

        private static Vector2 MixWithRandom(Vector2 vec, float urgency = 0f)
        {
            float r = Random.Range(0f, 1f);
            return Vector3.Slerp(
                vec,
                Random.onUnitCircle,
                Mathf.Lerp(r*r, 1f, urgency)
            );
        }

        private void OnEnable() => _eyes.enabled = true;

        private void OnDisable() => _eyes.enabled = false;

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