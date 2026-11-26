using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fishies.Movement.Navigation
{
    public class FishEyes : MonoBehaviour
    {
        public bool Spotted { get; private set; }
        public float Weight { get; private set; }
        public Vector2 AwayDirection { get; private set; }
        
        public float NearDistance { get; private set; }
        
        [SerializeField] private Transform eyes;
        [SerializeField] private float viewDistance = 2f;
        [SerializeField] private LayerMask layerMask;

        [Space]
        
        [SerializeField] private float fov = 120f;
        [SerializeField] private int eyeCount = 4;

        [Space]

        [SerializeField] private Range scanIntervalHz;

        private Transform _transform;
        private Transform _eyesTransform;

        private float _scanTimer;

        private void Awake()
        {
            _transform = transform;
            _eyesTransform = eyes;
        }

        public void Init(float sizeFactor)
        {
            viewDistance *= sizeFactor;
        }

        private void Update()
        {
            _scanTimer -= Time.deltaTime;
            if (_scanTimer > 0f) return;

            _scanTimer = 1f / scanIntervalHz.Random();

            See();
        }

        private void See()
        {
            if (eyeCount <= 0 || fov <= 0f || viewDistance <= 0f)
            {
                ClearDetection();
                return;
            }
            
            Vector2 source = _eyesTransform.position;
            Vector2 forward = _transform.right;

            float halfFov = fov * 0.5f;
            float angleStep = fov / eyeCount;
            
            DebugDrawer debugDrawer = DebugDrawer.Instance;
            bool debugEyes = debugDrawer && debugDrawer.EnableDebug && debugDrawer.Fish_Eyes;
            bool drawRaycasts = debugEyes && debugDrawer.Fish_Eyes_DrawRaycasts;
            bool drawDirections = debugEyes && debugDrawer.Fish_Eyes_DrawDirections;

            bool spotted = false;

            float closestDistance = float.MaxValue;
            Vector2 closestPoint = Vector2.zero;
            for (int i = 0; i <= eyeCount; i++)
            {
                float angle = -halfFov + angleStep * i;

                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * forward;
                if (drawRaycasts) Debug.DrawRay(source, direction * viewDistance, Color.red);

                RaycastHit2D hit = Physics2D.Raycast(source, direction, viewDistance, layerMask);
                if (!hit.collider) continue;

                spotted = true;
                if (drawRaycasts) Debug.DrawLine(source, hit.point, Color.white);

                if (hit.distance >= closestDistance) 
                    continue;

                closestDistance = hit.distance;
                closestPoint = hit.point;
            }

            Spotted = spotted;

            if (!spotted)
            {
                ClearDetection();
                return;
            }

            Vector2 awayDirection = source - closestPoint;

            NearDistance = awayDirection.magnitude;
            Weight = Mathf.Clamp01((viewDistance - NearDistance) / viewDistance);
            AwayDirection = awayDirection.normalized;

            if (drawDirections) Debug.DrawRay(source, AwayDirection * (Weight * 3f), Color.red);
        }

        private void ClearDetection()
        {
            Spotted = false;
            Weight = 0f;
            NearDistance = 0f;
            AwayDirection = Vector2.zero;
        }
    }
}