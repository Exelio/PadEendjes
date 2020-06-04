using View;
using Utils;
using UnityEngine;
using System;
using System.Collections;

namespace Model
{
    public class CameraEngine
    {
        public event Action<Mistakes> OnMistake;
        public event Action<string> OnLookedWell;
        public event Action<bool> OnCameraRotateToForward;

        private CameraStats _stats;

        private Vector3 _dollyDirection;

        private float _distance;
        private float _startAngle;

        private bool _isToggled;
        private bool _hasToWatchLeftAndRight = true;

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
            _stats = _view.Stats;
            _target = target;

            _query = new EnvironmentQuery();
        }

        public void FixedCameraUpdate(bool pauzed)
        {
            ToggleAnchorPoints();
             _stats = _view.Stats;

            if (!pauzed)
            {
                ObstacleCollision();
                CheckCorrectCrossing();
            }
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

            //if (!_isRotatingToForward)
            //{
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
            //}
        }

        float _waitTimeLeft;
        float _waitTimeRight;
        private void CheckLeftRight()
        {
            Debug.Log(_hasToWatchLeftAndRight);

            if (!_hasToWatchLeftAndRight) return;

            float angle = _view.transform.rotation.eulerAngles.y;
            CheckAngle(ref _hasLookedLeft, angle, _startAngle - 80f, ref _waitTimeLeft, "Links goed!");
            CheckAngle(ref _hasLookedRight, angle, _startAngle + 80f, ref _waitTimeRight, "Rechts goed!");

            if (_hasLookedRight && _hasLookedLeft) { _hasToWatchLeftAndRight = false; }
        }

        private void CheckAngle(ref bool looked, float angle, float startAngle, ref float timer, string text)
        {
            if (looked) return;

            float difference = Mathf.DeltaAngle(angle, startAngle);

            if (difference < 20f && difference > -20f)
            {
                timer += Time.deltaTime;
                if (CheckTimePast(timer))
                {
                    looked = true;
                    timer = 0;
                    OnLookedWell?.Invoke(text);
                }
            }
            else timer = 0;
        }

        private bool CheckTimePast(float time)
        {
            return time >= .75f;
        }

        public void ApplyRotation(float horizontal, float vertical)
        {
            //Debug.Log(_isRotatingToForward);

            //if (!_isRotatingToForward) return;

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

        private bool _isRotatingToForward;
        public void ToggleAnchorPoint(bool value)
        {
            if (_isToggled != value)
            {
                _isToggled = value;

                if (!_isToggled)
                {
                    _hasToWatchLeftAndRight = true;
                    //_view.StartCoroutine(StartChecking());
                    OnCameraRotateToForward?.Invoke(_isRotatingToForward);
                }
            }
        }

        private void ToggleAnchorPoints()
        {
            if (_isToggled)
                _stats.CameraTransform.SetParent(_stats.ForwardAnchorPoint);
            else
            {
                _hasToWatchLeftAndRight = true;
                _hasLookedLeft = false;
                _hasLookedRight = false;

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

        IEnumerator StartChecking()
        {
            float angle = Mathf.DeltaAngle(_stats.PrimaryAnchorPoint.rotation.y, _target.rotation.y);

            if (Mathf.Abs(angle) > 0.075f)
            {
                while (Mathf.Abs(angle) != 0)
                {
                    angle = Mathf.DeltaAngle(_stats.PrimaryAnchorPoint.rotation.y, _target.rotation.y);
                    _stats.PrimaryAnchorPoint.rotation = Quaternion.Lerp(_stats.PrimaryAnchorPoint.rotation, _target.rotation, _stats.FollowLerpSpeed);

                    if (Mathf.Abs(angle) < .075f) { _stats.PrimaryAnchorPoint.rotation = _target.rotation; break; }

                    if (!_isToggled)
                        break;
                    yield return null;
                }
            }
            else
            {
                _stats.PrimaryAnchorPoint.rotation = _target.rotation;
            }
            if (_isToggled)
            {
                _hasDoneMovingMistake = false;
                _hasLookedLeft = false;
                _hasLookedRight = false;
                _hasToWatchLeftAndRight = true;
            }

            _isRotatingToForward = false;
            _startAngle = _view.transform.rotation.eulerAngles.y;
            _previousPosition = _target.transform.position;
            OnCameraRotateToForward?.Invoke(_isRotatingToForward);
        }
    }
}