using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Fish.Navigation
{
    public static class NavQuery
    {
        public static bool IsWalkable(Vector2 point, float tolerance = 0.05f,
            int areaMask = NavMesh.AllAreas
        )
        {
            Vector3 worldPoint = new(point.x, point.y, 0f);

            if (!NavMesh.SamplePosition(worldPoint, out NavMeshHit hit, tolerance, areaMask))
                return false;

            Vector2 nearestPoint = new(hit.position.x, hit.position.y);

            return Vector2.Distance(point, nearestPoint) <= tolerance
                   && Mathf.Abs(hit.position.z - worldPoint.z) <= tolerance;
        }
        
        public static bool TryGetPath(Vector2 start, Vector2 destination, out Vector2[] corners,
            float endpointTolerance = 0.05f,
            int areaMask = NavMesh.AllAreas
        )
        {
            corners = System.Array.Empty<Vector2>();

            // Reject endpoints that aren't on or very near the NavMesh.
            if (!TryFindNavPoint(start, endpointTolerance, areaMask,
                    out Vector3 navStart) ||
                !TryFindNavPoint(destination, endpointTolerance, areaMask,
                    out Vector3 navDestination))
            {
                return false;
            }

            var path = new NavMeshPath();

            bool found = NavMesh.CalculatePath(
                navStart,
                navDestination,
                areaMask,
                path);

            // CalculatePath can return true for a partial path.
            if (!found || path.status != NavMeshPathStatus.PathComplete)
                return false;

            Vector3[] pathCorners = path.corners;
            corners = new Vector2[pathCorners.Length];

            for (int i = 0; i < pathCorners.Length; i++)
            {
                corners[i] = new Vector2(
                    pathCorners[i].x,
                    pathCorners[i].y);
            }

            return true;
        }

        private static bool TryFindNavPoint(Vector2 point, float tolerance, int areaMask, out Vector3 nearest)
        {
            nearest = default;

            Vector3 query = new(point.x, point.y, 0f);
            if (!NavMesh.SamplePosition(query, out NavMeshHit hit, tolerance, areaMask))
                return false;

            Vector2 sampledXY = new(hit.position.x, hit.position.y);
            if (Vector2.Distance(point, sampledXY) > tolerance ||
                Mathf.Abs(hit.position.z - query.z) > tolerance)
                return false;

            nearest = hit.position;
            return true;
        }
    }
}
