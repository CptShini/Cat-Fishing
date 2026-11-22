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
        
        public float NearDistance { get; private set; }
        public Vector2 CurrentDirection { get; private set; }
        private Vector2 _desiredDirection;
        private float _distanceToWaypoint;
        
        private bool _hasDestination;
        private Vector2 _destination;
        private readonly Queue<Vector2> _path = new();

        private bool _hasWaypoint;
        private Vector2 _waypoint;

        private bool _isFleeing;

        private int _areaMask;
        
        private FishEyes _eyes;
        [SerializeField] private Transform eyePoint;
        
        private float _arrivalDistanceTolerance;
        private Range _fleeDist;
        private Range _wanderDist;
        
        [SerializeField] private Range reNavTime;
        
        private Transform _transform;
        private Vector2 _position;

        [SerializeField] private Range navigateIntervalHz;

        private float _navigateTimer;
        private Coroutine _recalculateCoroutine;

        private void Awake()
        {
            _eyes = GetComponent<FishEyes>();
            _transform = transform;
            _position = _transform.position;
            
            CurrentDirection = Random.onUnitCircle;
        }

        public void Init(int biome, int speedStat)
        {
            _areaMask = biome;
            _arrivalDistanceTolerance = speedStat / 6f + 4f/3f;
            _wanderDist.Min = _arrivalDistanceTolerance * 1.5f;
            _wanderDist.Max = _arrivalDistanceTolerance * 3f;
            _fleeDist.Min = _wanderDist.Min * 2f;
            _fleeDist.Max = _wanderDist.Max * 2f;
        }

        private void Update()
        {
            _position = _transform.position;
                
            _navigateTimer -= Time.deltaTime;

            if (_navigateTimer <= 0f)
            {
                _navigateTimer = 1f / navigateIntervalHz.Random();
                Navigate();
            }

            if (!_hasDestination) return;

            NearDistance = !_eyes.Spotted 
                ? _distanceToWaypoint 
                : Mathf.Min(_distanceToWaypoint, _eyes.NearDistance);
            
            Vector2 targetDirection = !_eyes.Spotted
                ? _desiredDirection
                : Vector3.Slerp(_desiredDirection, _eyes.AwayDirection, _eyes.Weight);
            
            float t = 1f - Mathf.Exp(-5f * Time.deltaTime);
            CurrentDirection = Vector3.Slerp(CurrentDirection, targetDirection, t);
        }

        private void Navigate()
        {
            if (!_hasDestination) return;

            if (!_hasWaypoint && !SelectNextWaypoint())
            {
                FinishDestination();
                return;
            }

            _distanceToWaypoint = Vector2.Distance(_position, _waypoint);
            if (_distanceToWaypoint > _arrivalDistanceTolerance)
            {
                _desiredDirection = (_waypoint - _position).normalized;
                return;
            }
            
            _path.Dequeue();
            _hasWaypoint = false;

            if (!SelectNextWaypoint())
                FinishDestination();

            return;
            
            void FinishDestination()
            {
                _hasDestination = false;
                _desiredDirection = Vector2.zero;
                _isFleeing = false;
                _distanceToWaypoint = 99999f;

                if (_recalculateCoroutine != null)
                {
                    StopCoroutine(_recalculateCoroutine);
                    _recalculateCoroutine = null;
                }

                DestinationReached.Invoke();
            }
        }

        public void Wander()
        {
            if (_isFleeing) return;
            
            var path = Pathfind(0.5f, urgency =>
            {
                Vector2 newDirection = MixWithRandom(CurrentDirection, urgency);
                float wanderDistance = _wanderDist.Random();

                var hit = Physics2D.Raycast(eyePoint.position, newDirection, wanderDistance);
                Vector2 wanderTarget = hit.collider 
                    ? hit.point 
                    : _position + newDirection * wanderDistance;
                
                return wanderTarget;
            });
            
            SetPath(path);
        }
        
        public void Flee(Vector2 fleePoint)
        {
            if (_isFleeing) return;
            
            var path = Pathfind(_fleeDist.Max, urgency =>
            {
                Vector2 awayDir = (_position - fleePoint).normalized;
                Vector2 fleeDir = MixWithRandom(awayDir, urgency);
            
                float fleeDistance = _fleeDist.Random();
                Vector2 fleeTarget = _position + fleeDir * fleeDistance;
                return fleeTarget;
            });
            
            FilterFurthestPath();
            if (path.Length == 0) return;
            
            SetPath(path);
            _isFleeing = true;
            
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
                    float a = _position.x - p.x;
                    float b = _position.y - p.y;
                    return a * a + b * b;
                }
            }
        }

        private void SetPath(Vector2[] path)
        {
            if (path.Length == 0) return;
            
            _hasDestination = true;
            _destination = path[^1];
            
            ApplyPath(path);

            _recalculateCoroutine ??= StartCoroutine(RecalculatePath());
        }
        
        private void ApplyPath(Vector2[] path)
        {
            _path.Clear();
            foreach (Vector2 p in path)
                _path.Enqueue(p);

            SelectNextWaypoint();
        }
        
        private bool SelectNextWaypoint()
        {
            while (_path.Count > 0)
            {
                _waypoint = _path.Peek();
                
                _distanceToWaypoint = Vector2.Distance(_position, _waypoint);
                if (_distanceToWaypoint <= _arrivalDistanceTolerance)
                {
                    _path.Dequeue();
                    continue;
                }

                _hasWaypoint = true;
                _desiredDirection = (_waypoint - _position).normalized;
                return true;
            }

            _hasWaypoint = false;
            _distanceToWaypoint = 99999f;
            _desiredDirection = Vector2.zero;

            return false;
        }
        
        private IEnumerator RecalculatePath()
        {
            while (_hasDestination)
            {
                yield return new WaitForSeconds(reNavTime.Random());

                if (!_hasDestination) break;

                if (TryPathfind(_destination, 0.5f, out var path) && path is { Length: > 0 })
                {
                    ApplyPath(path);
                }
            }

            _recalculateCoroutine = null;
        }

        private Vector2[] Pathfind(float tolerance, Func<float, Vector2> randomDestinationFunc)
        {
            const int maxIterations = 25;
            for (int i = 0; i < maxIterations; i++)
            {
                float urgency = (float)i / maxIterations;
                bool validPath = TryPathfind(
                    randomDestinationFunc(urgency),
                    tolerance + urgency * urgency * urgency * maxIterations,
                    out var path
                );
                if (validPath) return path;
            }
            
            Debug.LogWarning($"{name} didn't find any path!");
            return Array.Empty<Vector2>();
        }
        
        private bool TryPathfind(Vector2 destination, float tolerance, out Vector2[] path) =>
            NavQuery.TryGetPath(_position, destination, out path, _areaMask, tolerance);

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
                Gizmos.DrawRay(_position, _desiredDirection);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(_position, CurrentDirection);
            }
            
            void DrawPath()
            {
                if (_path == null || _path.Count == 0)
                    return;
            
                Gizmos.color = Color.white;
                
                Vector2 prev = _position;
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