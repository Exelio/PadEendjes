using UnityEngine;

namespace Utils
{
    public class EnvironmentQuery
    {
        public RaycastHit HitInfo => _hitInfo;

        private RaycastHit _hitInfo;

        public bool IsGrounded(Vector3 origin, float radius, LayerMask groundLayer)
        {
            origin = new Vector3(origin.x, (origin.y + radius) - 0.02f, origin.z);

            return Physics.CheckSphere(origin, radius, groundLayer);
        }

        public bool IsColliding(Vector3 position, float radius)
        {
            return Physics.CheckSphere(position, radius);
        }

        public bool ShootRay(Vector3 origin, float distance, Vector3 direction, LayerMask streetLayer)
        {
            return Physics.Raycast(origin, direction, distance, streetLayer);
        }

        public bool CastSphere(Vector3 origin, float checkRadius, LayerMask crossingRoadLayer)
        {
            return Physics.CheckSphere(origin, checkRadius, crossingRoadLayer);
        }
    }
}