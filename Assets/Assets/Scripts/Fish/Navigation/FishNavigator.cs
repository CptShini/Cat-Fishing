using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fish.Navigation
{
    [RequireComponent(typeof(FishEyes))]
    public class FishNavigator : MonoBehaviour
    {
        public Vector2 DesiredDirection { get; private set; }
        public Vector2 CurrentDirection { get; private set; }

        private Vector2 _destination;
        
        private FishEyes _eyes;
        
        [SerializeField] private Transform target;
        [SerializeField] private float tolerance;
        [SerializeField] private int areaMask;
        [SerializeField] private float fleeDistance;

        private Vector2[] corners;
        private int navMask => 1 << areaMask;

        // targetDir * waverDir * avoidanceDir
        
        private void Update()
        {
            //_destination = target.position;
            //Vector2.Distance(transform.position, _destination);
            //RandomTarget();
            SetFleeTarget(target.position);
            NavQuery.TryGetPath(transform.position, _destination, out corners, navMask, tolerance);
        }

        private void RandomTarget()
        {
            Vector2 asd = (Vector2)transform.position + CurrentDirection + Random.insideUnitCircle * tolerance;
            NavQuery.TryGetPoint(asd, out _destination, navMask, tolerance);
            CurrentDirection = _destination - (Vector2)transform.position;
        }

        private void SetFleeTarget(Vector2 fleePoint)
        {
            Vector2 awayDir = ((Vector2)transform.position - fleePoint).normalized;
            Vector2 fleeTarget = (Vector2)transform.position + awayDir * fleeDistance;
            bool hit = NavQuery.TryGetPoint(fleeTarget, out _destination, navMask, fleeDistance);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, tolerance);
            if (corners == null || corners.Length == 0)
                return;
            
            Vector2 prev = transform.position;
            foreach (Vector2 corner in corners)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(corner, 0.25f);
                Gizmos.DrawLine(prev, corner);
                
                prev = corner;
            }
        }
    }
}