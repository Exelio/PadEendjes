using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class EnvironmentQueryConfig
    {
        public LayerMask GroundLayerMask;

        [HideInInspector]
        public Vector3 Origin;

        [HideInInspector]
        public float Radius;
        [HideInInspector]
        public float RaycastHeight;

        [HideInInspector]
        public bool IsGrounded;
    }

    public class EnvironmentQuery
    {
        public Vector3 GroundNormal => _groundNormal;

        private Vector3 _groundNormal;

        private readonly EnvironmentQueryConfig _config;

        public EnvironmentQuery(EnvironmentQueryConfig configuration)
        {
            _config = configuration;
        }

        public void FixedUpdate(Vector3 center)
        {
            //Vector3 groundPoint = FindGroundPoint();

            //_groundNormal = FindGroundNormal(groundPoint);

            _config.IsGrounded = IsGrounded(center);
        }

        //private Vector3 FindGroundPoint()
        //{
        //    Ray groundPointRay = new Ray(_center, Vector3.down);

        //    if (Physics.SphereCast(groundPointRay, _config.Radius, out RaycastHit hitInfo, _config.RaycastHeight, _config.GroundLayerMask))
        //        return hitInfo.point;

        //    return Vector3.zero;
        //}

        //private Vector3 FindGroundNormal(Vector3 groundPoint)
        //{
        //    Ray groundNormalRay = new Ray(new Vector3(groundPoint.x, _center.y, groundPoint.z), Vector3.down);

        //    if (Physics.Raycast(groundNormalRay, out RaycastHit hitInfo, _config.RaycastHeight))
        //        return hitInfo.normal;

        //    return Vector3.up;
        //}

        private bool IsGrounded(Vector3 center)
        {
            Vector3 origin = new Vector3(_config.Origin.x, _config.Origin.y + _config.Radius, _config.Origin.z);

            return Physics.CheckSphere(origin, _config.Radius, _config.GroundLayerMask);
        }
    }
}