using System.Collections.Generic;
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

        public bool ShootRay(ref RaycastHit hitinfo, Vector3 origin, float distance, Vector3 direction, LayerMask streetLayer)
        {
            return Physics.Raycast(origin, direction, out hitinfo, distance, streetLayer);
        }

        public bool CastSphere(Vector3 origin, float checkRadius, LayerMask crossingRoadLayer)
        {
            return Physics.CheckSphere(origin, checkRadius, crossingRoadLayer);
        }

        public float CheckDotProduct(Transform self, Transform target)
        {
            return Vector3.Dot(self.forward, target.forward);
        }

        public float CheckAngle(Vector3 target, Vector3 self)
        {
            return Vector3.Angle(self, target);
        }

        public void FindVisibleTargets(ref List<Transform> transformList, Transform origin, float radius, float viewAngle,LayerMask targetMask, LayerMask obstacleMask)
        {
            transformList.Clear();
            Collider[] targets = Physics.OverlapSphere(origin.position, radius, targetMask);

            foreach (var target in targets)
            {
                Transform targetTrans = target.transform;
                Vector3 dirToTarget = (targetTrans.position - origin.position).normalized;

                if (Vector3.Angle(origin.forward, dirToTarget) < viewAngle / 2)
                {
                    float disToTarget = Vector3.Distance(origin.position, targetTrans.position);

                    if (!Physics.Raycast(origin.position, dirToTarget, disToTarget, obstacleMask) && targetTrans != origin)
                        transformList.Add(targetTrans);
                }
            }
        }
    }
}