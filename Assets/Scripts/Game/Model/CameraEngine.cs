using View;
using Utils;
using UnityEngine;
using System;

namespace Model
{
    public class CameraEngine
    {
        public event Action<Mistakes> OnMistake;
        public event Action<string> OnLookedWell;
        private CameraStats _stats;

        private Vector3 _dollyDirection;

        private float _distance;
        private float _startAngle;

        private bool _isToggled;
        private bool _hasToWatchLeftAndRight;

        private bool _hasDoneMovingMistake;
        private bool _hasLookedLeft = false;
        private bool _hasLookedRight = false;

        private readonly Transform _target;
        private Vector3 _previousPosition;
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
            if (!_isToggled)
            {
                _previousPosition = _target.transform.position;
                _startAngle = _view.transform.rotation.eulerAngles.y;
                _mistakeCrossAtUnsaveSpot = false;
                return; 
            }

            CheckDistanceChange();
        }

        public void SetCrossUnsaveSpot(bool value)
        {
            _mistakeCrossAtUnsaveSpot = value;
        }

        private bool _mistakeCrossAtUnsaveSpot;
        private void CheckDistanceChange()
        {
            if (_mistakeCrossAtUnsaveSpot) return;

            float distance = Vector3.Distance(_target.transform.position, _previousPosition);

            if (distance > 1.5f && _hasToWatchLeftAndRight && !_hasDoneMovingMistake)
            {
                _hasDoneMovingMistake = true;
                OnMistake?.Invoke(Mistakes.NotLookingLeftAndRight);
                _previousPosition = _target.transform.position;
            }
            else
            {
                CheckLeftRight();
            }
        }
        private void CheckLeftRight()
        {
            if (!_hasToWatchLeftAndRight) return;

            float angle = _view.transform.rotation.eulerAngles.y;
            CheckLeft(angle);
            CheckRight(angle);
            if (_hasLookedRight && _hasLookedLeft) { _hasToWatchLeftAndRight = false; }
        }

        private void CheckLeft(float angle)
        {
            if (_hasLookedLeft) return;

            float differnce = Mathf.DeltaAngle(angle, _startAngle - 80f);

            if (differnce < 10f && differnce > -10f)
            {
                _hasLookedLeft = true;
                OnLookedWell?.Invoke("Links goed!");
            }
        }

        private void CheckRight(float angle)
        {
            if (_hasLookedRight) return;

            float differnce = Mathf.DeltaAngle(angle, _startAngle + 80f);

            if (differnce < 10f && differnce > -10f)
            {
                _hasLookedRight = true;
                OnLookedWell?.Invoke("Rechts goed!");
            }
        }

        public void ApplyRotation(float horizontal, float vertical)
        {
            Vector3 forward = _stats.PrimaryAnchorPoint.forward;
            Vector3 up = _stats.ForwardAnchorPoint.forward;

            float forwardAngleInDegrees = Mathf.Sin(forward.y) * Mathf.Rad2Deg;
            float upAngleInDegrees = Mathf.Tan(up.z) * Mathf.Rad2Deg;

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
            if (_isToggled != value)
            {
                _isToggled = value;

                if (_isToggled)
                {
                    _hasDoneMovingMistake = false;
                    _hasLookedLeft = false;
                    _hasLookedRight = false;
                    _hasToWatchLeftAndRight = true;
                }
            }
        }

        private void ToggleAnchorPoints()
        {
            if (_isToggled)
                _stats.CameraTransform.SetParent(_stats.ForwardAnchorPoint);
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

            _stats.BackwardAnchorPoint.localPosition = Vector3.Lerp(_stats.BackwardAnchorPoint.localPosition, _dollyDirection * _distance, _stats.ClipLerpSpeed * Time.deltaTime);
        }
    }
}