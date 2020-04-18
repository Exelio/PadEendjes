using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficController
{
    private VehicleView _view; //view of the vehicle

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
    private bool _behindVehicle = false;

    private Waypoint _destinationWP;
    private Transform _destinationTrans;

    [SerializeField] private float _checkSpeed; public float CheckSpeed => _checkSpeed;
    [SerializeField] private float _currentSpeed; public float CurrentSpeed => _currentSpeed;

    private List<Collider> _hitList = new List<Collider>();
    private TrafficWaypointNavigator _navigator;

    public TrafficController(VehicleView view)
    {
        _view = view;
        _navigator = new TrafficWaypointNavigator(this, _view.ViewVariables.StartWaypoint);
        Initialize();
    }

    public void Initialize()
    {
        _navigator.Initialize();
        SetPosition();
        _checkSpeed = _view.ViewVariables.MaxSpeed;
    }

    private void SetPosition()
    {
        Transform wpTrans = _wp.transform;
        _view.transform.position = wpTrans.position;
        Vector3 degrees = wpTrans.localRotation.eulerAngles;
        _view.transform.rotation = Quaternion.Euler(degrees);
        _wp = _wp.NextWaypoint;
    }

    public void Update()
    {
        _navigator.Update();
        if (_wp != null)
        {
            CheckDestinationReached();           
            CheckForward();
        }
        else { _checkSpeed = 0; _currentSpeed = 0;}
    }

    public void FixedUpdate()
    {
        if (_wp != null) Move();
    }

    private void Move()
    {
        Vector3 destinationDirection = _wp.transform.position - _view.transform.position;
        destinationDirection.y = 0;
        _currentSpeed = _view.ViewVariables.RigidBody.velocity.magnitude;

        ChangeCurrentSpeed();

        Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
        var slerp = Quaternion.Slerp(_view.transform.rotation, targetRotation, _view.ViewVariables.RotationSpeed * Time.deltaTime);
        _view.transform.rotation = slerp;
    }

    private void ChangeCurrentSpeed()
    {
        if (Math.Abs(_checkSpeed - _currentSpeed) <= .5f) 
        { 
            _currentSpeed = _checkSpeed;
            _view.transform.position += _view.transform.forward * Time.deltaTime * _currentSpeed; 
        }
        else if (_currentSpeed <= _checkSpeed) _view.ViewVariables.RigidBody.AddForce(_view.transform.forward * _view.ViewVariables.AccelerationSpeed, ForceMode.Force);
    }

    public void ChangeCheckSpeed()
    {
        _checkSpeed = _wp.MaxSpeed;
    }

    private void CheckDestinationReached()
    {
        if (Vector3.Distance(_view.transform.position, _wp.transform.position) <= _view.ViewVariables.Distance) _reachedDestination = true;
        else  _reachedDestination = false;
    }

    private void CheckForward()
    {
        Checkcast(out _hitList);

        if (_hitList.Count > 0)
        {
            foreach (var item in _hitList)
            {
                if(item.GetComponent<VehicleView>() != null && item.name != _view.name)
                {
                    CheckHit(item.GetComponent<VehicleView>());
                }
            }
        }

        ChangeSpeed();

        if (!_behindVehicle) _checkSpeed = _wp.PreviousWaypoint.MaxSpeed;

        _hitList.Clear();
        _behindVehicle = false;
    }

    private void CheckHit(VehicleView view)
    {
        if (view == null) return;

        _behindVehicle = true;
        _view.Speed = view.Speed - 0.5f;
    }

    private void Checkcast(out List<Collider> hitList)
    {
        //hitDetection = Physics.BoxCast(_carFront.position + (transform.forward * (_maxDistance / 2)), _carFront.localScale, transform.forward, out hit, _carFront.rotation, _maxDistance);
        //hitDetection = Physics.SphereCast(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance, transform.forward, out hit);
        //hitDetection = Physics.CheckSphere(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance, _checkLayer);

        Collider[] hits = Physics.OverlapSphere(_view.ViewVariables.CarFront.position + (_view.transform.forward * _view.ViewVariables.MaxDistance / 2), _view.ViewVariables.MaxDistance, _view.ViewVariables.CheckLayer);
        hitList = hits.ToList();
    }

    private bool CheckAngle(Transform t)
    {
        //float angle = Vector3.Angle(t.localRotation.eulerAngles, transform.localRotation.eulerAngles);
        //return angle <= _checkAngle;
        return true;
    }

    private void ChangeSpeed()
    {
        if (_behindVehicle)
        {
            if (_view.Speed <= 0.1f) _checkSpeed = 0;

            else _checkSpeed = _view.Speed;
        }

        _checkSpeed = Mathf.Clamp(_checkSpeed, 0, _view.ViewVariables.MaxSpeed);

        _view.Speed = _checkSpeed;
    }
}