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

        public bool IsColliding(Vector3 startPoint, Vector3 endPoint)
        {
            return Physics.Linecast(startPoint, endPoint, out _hitInfo);
        }
    }
}