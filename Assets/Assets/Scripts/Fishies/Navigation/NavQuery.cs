using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Fishies.Navigation
{
    public static class NavQuery
    {
        private const float DefaultTolerance = 0.05f;
        
        public static bool TryGetPoint(Vector2 point,
            out Vector2 nearestPoint,
            int areaMask = NavMesh.AllAreas,
            float tolerance = DefaultTolerance
        ) {
            nearestPoint = default;
            
            bool success = TryFindNavPoint(point, tolerance, areaMask, out Vector3 nearest);
            if (!success) return false;
            
            nearestPoint = nearest;
            return true;
        }

        public static bool TryGetPath(Vector2 start, Vector2 destination,
            out Vector2[] corners,
            int areaMask = NavMesh.AllAreas,
            float endpointTolerance = DefaultTolerance
        ) {
            corners = System.Array.Empty<Vector2>();
            
            if (!TryFindNavPoint(start, endpointTolerance, areaMask, out Vector3 navStart) ||
                !TryFindNavPoint(destination, endpointTolerance, areaMask, out Vector3 navDestination))
                return false;

            NavMeshPath path = new();
            bool found = NavMesh.CalculatePath(
                navStart,
                navDestination,
                areaMask,
                path
            );
            
            if (!found || path.status != NavMeshPathStatus.PathComplete)
                return false;

            corners = path.corners.Skip(1).Select(c => new Vector2(c.x, c.y)).ToArray();
            return true;
        }

        private static bool TryFindNavPoint(Vector2 point, float tolerance, int areaMask,
            out Vector3 nearest
        ) {
            nearest = default;

            Vector3 query = new(point.x, point.y, 0f);
            if (!NavMesh.SamplePosition(query, out NavMeshHit hit, tolerance, areaMask))
                return false;

            Vector2 sampledXY = new(hit.position.x, hit.position.y);
            if (Vector2.Distance(point, sampledXY) > tolerance)
                return false;

            nearest = hit.position;
            return true;
        }
    }
}
