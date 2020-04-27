using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController
{
    #region Variables

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

    #endregion

    public TrafficController(VehicleView view)
    {
        _view = view;
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

    public void Update()
    {
        _navigator.Update();
        if (_wp != null)
        {
            CheckDestinationReached();
            CheckForward();
        }
        else 
        { 
            _speed = 0; 
            _currentSpeed = 0;
            CheckDestroy();
        }
    }

    public void FixedUpdate()
    {
        if (_wp != null) Move();
    }

    #endregion

    private void CheckDestinationReached()
    {
        if (Vector3.Distance(_view.transform.position, _wp.transform.position) <= _view.ViewVariables.Distance) _reachedDestination = true;
        else  _reachedDestination = false;
    }

    private void CheckDestroy()
    {
        if (!IsStopping)
        {
            OnDestroy?.Invoke(this);
            _view.DestroyVehicle();
        }
    }
    
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
        else if (_speed <= _view.CheckSpeed) _view.ViewVariables.RigidBody.AddForce(_view.transform.forward * _view.ViewVariables.AccelerationSpeed, ForceMode.Force);
    }

    private void Rotate(Vector3 destinationDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
        var slerp = Quaternion.Slerp(_view.transform.rotation, targetRotation, _view.ViewVariables.RotationSpeed * Time.deltaTime);
        _view.transform.rotation = slerp;
    }

    private void CheckForward()
    {
        if (_visibleTargets.Count <= 0) { ChangeCheckSpeed(_wp.PreviousWaypoint.MaxSpeed); return; }

        foreach (var visibleTarget in _visibleTargets)
        {
            VehicleView view = visibleTarget.GetComponent<VehicleView>();
            float distance = Vector3.Distance(_view.transform.position, visibleTarget.transform.position);
            float forwardAngle = CheckAngle(visibleTarget.transform.forward, _view.transform.forward);
            float angle = CheckAngle(visibleTarget.transform.position, _view.transform.position);
            float dotResult = CheckDotProduct(visibleTarget);
            bool checkAngles = CheckAngles(forwardAngle, _variables.AngleMaxMin.x, _variables.AngleMaxMin.y, dotResult);

            //Debug.Log($"Distance between {_view.name} and {visibleTarget.name} = {distance}, angle = {angle}");
            if (view != null)
            {
                CheckCarDirection(view, forwardAngle, dotResult);
            }
            else if (distance <= _variables.PedestrianDistance && checkAngles)
            {
                //Debug.Log($"Player wants to cross road = true");
                _isCrossingRoad = true;
                ChangeCheckSpeed(0f);
                SetVelocityToZero();
            }
            else if (_isCrossingRoad && angle < 75 && angle > -75)
            {
                //Debug.Log($"Player is crossing road = true");
                ChangeCheckSpeed(0f);
                SetVelocityToZero();
            }
            else
            {
                ChangeCheckSpeed(_wp.PreviousWaypoint.MaxSpeed);
                _isCrossingRoad = false;
            }

            if (!checkAngles && distance >= _variables.PedestrianDistance)
            {
                //Debug.Log($"Player wants to cross road = false");
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
        Debug.Log($"angle between {_view.name} and {view.name} = {angle}");
        if (angle < 25 && angle > -25)
        {
            Debug.Log("Car in front");
            ChangeCheckSpeed(view.CheckSpeed);

            if (Vector3.Distance(_view.transform.position, view.transform.position) <= _variables.VehicleMaxDistance)
            {
                Debug.Log("Car in front");
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

    private void SetVelocityToZero()
    {
        _variables.RigidBody.velocity = new Vector3(0, 0, 0);
    }

    private float CheckDotProduct(Transform trans)
    {
        return Vector3.Dot(_view.transform.forward, trans.transform.forward);
    }

    private float CheckAngle(Vector3 target, Vector3 self)
    {
        return Vector3.Angle(self, target);
    }

    private void FindVisibleTargets()
    {
        _visibleTargets.Clear();
        Collider[] targets = Physics.OverlapSphere(_variables.CarFront.position, _variables.ViewRadius, _variables.TargetMask);
        
        foreach (var target in targets)
        {
            Transform targetTrans = target.transform;
            Vector3 dirToTarget = (targetTrans.position - _view.transform.position).normalized;

            if (Vector3.Angle(_view.transform.forward, dirToTarget) < _variables.ViewAngle / 2)
            {
                float disToTarget = Vector3.Distance(_view.transform.position, targetTrans.position);

                if (!Physics.Raycast(_view.transform.position + _variables.CarFront.position, dirToTarget, disToTarget, _variables.ObstacleMask) && targetTrans != _view.transform) 
                    _visibleTargets.Add(targetTrans);
            }
        }
    }

    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }    
}