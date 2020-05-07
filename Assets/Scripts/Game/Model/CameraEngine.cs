using View;
using Utils;
using UnityEngine;
using System;

namespace Model
{
    public class CameraEngine
    {
        private CameraStats _stats;

        private Vector3 _dollyDirection;

        private float _distance;

        private bool _isToggled;

        private readonly Transform _target;

        private readonly CameraView _view;

        private readonly EnvironmentQuery _query;

        public CameraEngine(CameraView view, Transform target)
        {
            _view = view;
            _target = target;

            _query = new EnvironmentQuery();
        }

        public void FixedCameraUpdate()
        {
            _stats = _view.Stats;

            ToggleAnchorPoints();
            ObstacleCollision();

            CheckCorrectCrossing();
        }

        private void CheckCorrectCrossing()
        {
            if (!_isToggled) return;

            bool hit = _query.ShootRay(_stats.CameraTransform.position, Mathf.Infinity, _stats.CameraTransform.forward, _stats.VehicleWindowLayer);
            //Debug.Log(hit);
        }

        public void ApplyRotation(float horizontal, float vertical)
        {
            Vector3 forward = _stats.PrimaryAnchorPoint.forward;
            Vector3 up = _stats.ForwardAnchorPoint.forward;

            float forwardAngleInDegrees = Mathf.Sin(forward.y) * Mathf.Rad2Deg;
            float upAngleInDegrees = Mathf.Tan(up.z) * Mathf.Rad2Deg;

            //_stats.ForwardAnchorPoint.rotation = Quaternion.Slerp(_stats.ForwardAnchorPoint.rotation, _target.rotation, Time.deltaTime);

            //if (_isToggled)
            //{
            //    if (horizontal > 0 && upAngleInDegrees > _stats.YawClamp.x)
            //        _stats.ForwardAnchorPoint.Rotate(Vector3.up, horizontal * _stats.RotationSpeed * Time.deltaTime, Space.World);
            //    else if (horizontal < 0 && upAngleInDegrees < _stats.YawClamp.y)
            //        _stats.ForwardAnchorPoint.Rotate(Vector3.up, horizontal * _stats.RotationSpeed * Time.deltaTime, Space.World);
            //}
            //else
            //  _stats.PrimaryAnchorPoint.Rotate(Vector3.up, horizontal * _stats.RotationSpeed * Time.deltaTime, Space.World);

            _stats.PrimaryAnchorPoint.Rotate(Vector3.up, horizontal * _stats.RotationSpeed * Time.deltaTime, Space.World);

            if (_stats.IsVerticalInverted)
                vertical *= -1;

            if (vertical > 0 && forwardAngleInDegrees > _stats.PitchClamp.x)
                _stats.PrimaryAnchorPoint.Rotate(Vector3.right, vertical * _stats.RotationSpeed * Time.deltaTime, Space.Self);
            else if (vertical < 0 && forwardAngleInDegrees < _stats.PitchClamp.y)
                _stats.PrimaryAnchorPoint.Rotate(Vector3.right, vertical * _stats.RotationSpeed * Time.deltaTime, Space.Self);
        }

        public void ToggleAnchorPoint(bool value)
        {
            if(_isToggled != value)
                _isToggled = value;
        }

        private void ToggleAnchorPoints()
        {
            if (_isToggled)
            {
                _stats.CameraTransform.SetParent(_stats.ForwardAnchorPoint);
            }
            else
            {
                _stats.CameraTransform.SetParent(_stats.BackwardAnchorPoint);

                _dollyDirection = _stats.BackwardAnchorPoint.localPosition.normalized;
                _distance = _stats.BackwardAnchorPoint.localPosition.magnitude;
            }

            _stats.PrimaryAnchorPoint.position = Vector3.Lerp(_stats.PrimaryAnchorPoint.position, _target.position + _stats.Offset, _stats.FollowLerpSpeed);
            _stats.CameraTransform.localPosition = Vector3.Lerp(_stats.CameraTransform.localPosition, Vector3.zero, _stats.SwitchLerpSpeed);
        }

        private void ObstacleCollision()
        {
            Vector3 desiredPosition = _stats.CameraTransform.parent.TransformPoint(_dollyDirection * _stats.MaximumDistance);

            bool isColliding = _query.IsColliding(_stats.BackwardAnchorPoint.position, _stats.CollisionRadius);

            if (isColliding)
                _distance = Mathf.Clamp(_query.HitInfo.distance, _stats.MinimumDistance, _stats.MaximumDistance);
            else
                _distance = _stats.MaximumDistance;

            //Debug.Log(isColliding);

            _stats.BackwardAnchorPoint.localPosition = Vector3.Lerp(_stats.BackwardAnchorPoint.localPosition, _dollyDirection * _distance, _stats.ClipLerpSpeed * Time.deltaTime);
        }
    }
}