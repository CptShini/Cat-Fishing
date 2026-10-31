using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private float tolerance;
        
        [Space]
        
        [SerializeField] private string areaMaskName;
        [SerializeField] private float arrivalDistanceTolerance;
        [SerializeField] private float minFleeDist, maxFleeDist;
        
        // targetDir * waverDir * avoidanceDir

        private void Start()
        {
            areaMask = 1 << NavMesh.GetAreaFromName(areaMaskName);
            TryFlee(target.position);
        }
        
        private void Update()
        {
            Pathfind();
        }

        private void Pathfind()
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
        }

        public bool TryFlee(Vector2 fleePoint)
        {
            Vector2 awayDir = ((Vector2)transform.position - fleePoint).normalized;
            
            float fleeDistance = Random.Range(minFleeDist, maxFleeDist);
            Vector2 fleeTarget = (Vector2)transform.position + awayDir * fleeDistance;
            
            bool validPath = NavQuery.TryGetPath(transform.position, fleeTarget, out Vector2[] path, areaMask, fleeDistance);
            if (!validPath) return false;
            
            FilterFurthestPath();
            SetPath(path);
            
            return true;
            
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
            _hasDestination = true;
            _destination = path[^1];
            
            _path.Clear();
            foreach (Vector2 p in path)
                _path.Enqueue(p);
        }
    }
}