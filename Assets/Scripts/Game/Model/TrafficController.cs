using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using View;

namespace Model
{
    public class TrafficController
    {
        #region Variables

        public event Action<Mistakes> OnMistake;
        public event Action<TrafficController> OnDestroy;
        public bool IsStopping { get; set; }

        private Waypoint _wp;
        public Waypoint Waypoint
        {
            get => _wp;
            set
            {
                _wp = value;
            }
        }

        private bool _reachedDestination = false; public bool ReachedDestination => _reachedDestination;
        private bool _isCrossingRoad = false;

        private Waypoint _destinationWP;
        private Transform _destinationTrans;
        private List<Transform> _visibleTargets = new List<Transform>();

        private float _speed; public float Speed => _speed;
        private float _currentSpeed; public float CurrentSpeed => _currentSpeed;

        private TrafficWaypointNavigator _navigator;
        private VehicleVariables _variables;
        private VehicleView _view; //view of the vehicle
        private readonly AudioManager _audioManager;
        private EnvironmentQuery _query;

        #endregion

        public TrafficController(VehicleView view, AudioManager audioManager)
        {
            _view = view;
            _audioManager = audioManager;
            _query = new EnvironmentQuery();
            _variables = view.ViewVariables;
            _navigator = new TrafficWaypointNavigator(this, _view.StartWaypoint);
            Initialize();
        }

        #region Initialize

        private void Initialize()
        {
            _navigator.Initialize();
            SetPosition();
            _speed = _view.ViewVariables.MaxSpeed;
            PlaySound();
        }

        private void SetPosition()
        {
            Transform wpTrans = _wp.transform;
            _view.transform.position = wpTrans.position;
            _wp = _wp.NextWaypoint;
            ChangeCheckSpeed(_wp.PreviousWaypoint.MaxSpeed);
        }

        #endregion

        public void ChangeCheckSpeed(float speed = 6.5f)
        {
            if (_wp != null)
            {
                _view.CheckSpeed = speed;
                _view.CheckSpeed = Mathf.Clamp(_view.CheckSpeed, 0, _variables.MaxSpeed);
            }
        }

        #region Updates

        public void FixedUpdate()
        {
            _navigator.Update();
            if (_wp != null)
            {
                Move();
                DestinationReached();
                CheckForward();
            }
            else
            {
                _speed = 0;
                _currentSpeed = 0;
                CheckDestroy();
            }
        }

        private void PlaySound()
        {
            _audioManager.Play("CarDriving", _variables.AudioSource);
        }

        #endregion

        public void OnPauze()
        {
            _variables.RigidBody.velocity = Vector3.zero;
        }

        public void OnResume()
        {
            _variables.RigidBody.velocity = Vector3.forward;
        }

        private void DestinationReached()
        {
            if (Vector3.Distance(_view.transform.position, _wp.transform.position) <= _view.ViewVariables.Distance) _reachedDestination = true;
            else _reachedDestination = false;
        }

        private void CheckDestroy()
        {
            if (!IsStopping)
            {
                OnDestroy?.Invoke(this);
                _view.DestroyVehicle();
            }
        }

        #region Movement

        private void Move()
        {
            Vector3 destinationDirection = _wp.transform.position - _view.transform.position;
            destinationDirection.y = 0;
            _speed = _variables.RigidBody.velocity.magnitude;

            ChangeCurrentSpeed();
            Rotate(destinationDirection);
        }

        private void ChangeCurrentSpeed()
        {
            if (Math.Abs(_view.CheckSpeed - _speed) <= .5f)
            {
                _speed = _view.CheckSpeed;
                _view.transform.position += _view.transform.forward * Time.deltaTime * _currentSpeed;
            }
            else if (_speed <= _view.CheckSpeed)
            {
                _view.ViewVariables.RigidBody.AddForce(_view.transform.forward * _view.ViewVariables.AccelerationSpeed, ForceMode.Force);
            }
        }

        private void Rotate(Vector3 destinationDirection)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
            var slerp = Quaternion.Slerp(_view.transform.rotation, targetRotation, _view.ViewVariables.RotationSpeed * Time.deltaTime);
            _view.transform.rotation = slerp;
        }

        #endregion

        private bool _startChecking = false;
        public void ToggleForwardChecking(bool value)
        {
            if (_startChecking == value) return;
            _startChecking = value;
        }

        private void SetVelocityToZero()
        {
            _variables.RigidBody.velocity = new Vector3(0, 0, 0);
        }

        public IEnumerator FindTargetWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                _query.FindVisibleTargets(ref _visibleTargets, _variables.CarFront, _variables.ViewRadius, _variables.ViewAngle, _variables.TargetMask, _variables.ObstacleMask);
            }
        }

        private void CheckForward()
        {
            if (_visibleTargets.Count <= 0) { ChangeCheckSpeed(_wp.PreviousWaypoint.MaxSpeed); return; }

            foreach (var visibleTarget in _visibleTargets)
            {
                VehicleView targetView = visibleTarget.GetComponent<VehicleView>();

                float distance = Vector3.Distance(_view.transform.position, visibleTarget.transform.position);

                float forwardAngle = _query.CheckAngle(visibleTarget.transform.forward, _view.transform.forward);
                float angle = Mathf.DeltaAngle(visibleTarget.transform.position.y, _view.transform.position.y);
                float dotResult = _query.CheckDotProduct(_view.transform, visibleTarget);

                bool checkAngles = CheckAngles(forwardAngle, _variables.AngleMaxMin.x, _variables.AngleMaxMin.y, dotResult);

                if (targetView != null)
                {
                    CheckCarDirection(targetView, forwardAngle, dotResult);
                }

                if (!_startChecking)
                {
                    if (_isCrossingRoad)
                    {
                        ChangeCheckSpeed(_wp.PreviousWaypoint.MaxSpeed);
                        _isCrossingRoad = false;
                    }
                    return; 
                }

                if (distance <= _variables.PedestrianDistance && checkAngles)
                {
                    _isCrossingRoad = true;
                    ChangeCheckSpeed(0f);
                    SetVelocityToZero();
                }
                else if (_isCrossingRoad && angle < _variables.PedestrianCrossingAngle && angle > -_variables.PedestrianCrossingAngle)
                {
                    ChangeCheckSpeed(0f);
                    SetVelocityToZero();
                }
                else if (!_isCrossingRoad && angle < _variables.PedestrianInFrontAngle && angle > -_variables.PedestrianInFrontAngle)
                {
                    _isCrossingRoad = true;
                    ChangeCheckSpeed(0f);
                    SetVelocityToZero();
                }

                if (!checkAngles && distance >= _variables.PedestrianDistance)
                {
                    _isCrossingRoad = false;
                }
            }
        }

        private bool CheckAngles(float angle, float maxAngle, float minAngle, float dotResult)
        {
            return (angle < maxAngle && angle > minAngle && dotResult < 0) || (angle < -minAngle && angle > -maxAngle && dotResult > 0);
        }

        private void CheckCarDirection(VehicleView view, float angle, float dotResult)
        {
            Debug.Log($"angle between {_view.name}, and {view.name} = {angle}");
            if (angle < 25 && angle > -25 && Vector3.Distance(_view.transform.position, view.transform.position) <= _variables.VehicleMinDistance)
            {
                ChangeCheckSpeed(view.CheckSpeed);

                if (Vector3.Distance(_view.transform.position, view.transform.position) <= _variables.VehicleMaxDistance)
                {
                    ChangeCheckSpeed(0f);
                    SetVelocityToZero();
                }
            }
            else if (dotResult > 0 && angle < _variables.AngleMaxMin.x && angle > _variables.AngleMaxMin.y)
            {
                ChangeCheckSpeed(0f);
                SetVelocityToZero();
            }
        }
    }
}