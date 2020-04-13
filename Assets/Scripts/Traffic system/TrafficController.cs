using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrafficController : MonoBehaviour
{
    [Header("Movement variables")]
    [Tooltip("The acceleration of the vehicle")] [SerializeField] private float _accelerationSpeed;
    [Range(1, 8)] [Tooltip("Max car speed")] [SerializeField] private float _maxSpeed; public float Speed => _maxSpeed;
    [Tooltip("The speed the car turns")] [SerializeField] private float _rotationSpeed;
    [Range(0,2)] [Tooltip("Distance until change waypoint")] [SerializeField] private float _distance = 0.1f;
    [Tooltip("The distance between the cars")] [SerializeField] private float _maxDistance = 2f;
    [Tooltip("The max angle between the car in front and this before it isn.t in front anymore")][SerializeField] private float _checkAngle = 50f;
    [Tooltip("Vehicle front position")] [SerializeField] private Transform _carFront;
    [SerializeField] private LayerMask _checkLayer;

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

    private Rigidbody _rigid;

    private Waypoint _destinationWP;
    private Transform _destinationTrans;

    [SerializeField] private float _checkSpeed; public float CheckSpeed => _checkSpeed;
    [SerializeField] private float _currentSpeed; public float CurrentSpeed => _currentSpeed;

    private float speed = 0;
    private List<Collider> _hitList = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        SetPosition();

        _checkSpeed = _maxSpeed;
        _rigid = GetComponent<Rigidbody>();
    }

    private void SetPosition()
    {
        Transform wpTrans = _wp.transform;
        transform.position = wpTrans.position;
        Vector3 degrees = wpTrans.localRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(degrees);
        _wp = _wp.NextWaypoint;

        //Debug.Log($"{this.name} \n waypoint transform: \n position = {wpTrans.position}, rotation = {wpTrans.rotation.eulerAngles}. \n car transform: \n position = {transform.position}, rotation = {transform.rotation.eulerAngles}");
    }

    // Update is called once per frame
    private void Update()
    {
        if (_wp != null)
        {
            CheckDestinationReached();           
            CheckForward();
        }
        else { _checkSpeed = 0; _currentSpeed = 0;}
    }

    private void FixedUpdate()
    {
        if (_wp != null) Move();
    }

    private void Move()
    {
        Vector3 destinationDirection = _wp.transform.position - transform.position;
        destinationDirection.y = 0;
        _currentSpeed = _rigid.velocity.magnitude;

        ChangeCurrentSpeed();

        Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
        var slerp = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.rotation = slerp;
    }

    private void ChangeCurrentSpeed()
    {
        if (Math.Abs(_checkSpeed - _currentSpeed) <= .5f) 
        { 
            _currentSpeed = _checkSpeed; 
            transform.position += transform.forward * Time.deltaTime * _currentSpeed; 
        }
        else if (_currentSpeed <= _checkSpeed) _rigid.AddForce(transform.forward * _accelerationSpeed, ForceMode.Force);
    }

    public void ChangeCheckSpeed()
    {
        _checkSpeed = _wp.MaxSpeed;
    }

    private void CheckDestinationReached()
    {
        if (Vector3.Distance(transform.position, _wp.transform.position) <= _distance) _reachedDestination = true;
        else  _reachedDestination = false;
    }

    private void CheckForward()
    {
        Checkcast(out _hitList);

        if (_hitList.Count > 0)
        {
            foreach (var item in _hitList)
            {
                if(item.GetComponent<TrafficController>() != null && item.name != this.name)
                {
                    CheckHit(item.GetComponent<TrafficController>());
                }
            }
        }

        ChangeSpeed();

        if (!_behindVehicle) _checkSpeed = _wp.PreviousWaypoint.MaxSpeed;

        _hitList.Clear();
        _behindVehicle = false;
    }

    private void CheckHit(TrafficController controller)
    {
        if (controller == null) return;

        if (CheckAngle(controller.gameObject.transform))
        {
            _behindVehicle = true;

            speed = controller.speed - 0.5f;
        }
        else _behindVehicle = false;
    }

    private void Checkcast(out List<Collider> hitList)
    {
        //hitDetection = Physics.BoxCast(_carFront.position + (transform.forward * (_maxDistance / 2)), 
        //    _carFront.localScale, transform.forward, out hit, _carFront.rotation, _maxDistance);

        //hitDetection = Physics.SphereCast(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance, transform.forward, out hit);
        //hitDetection = Physics.CheckSphere(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance, _checkLayer);
        Collider[] hits = Physics.OverlapSphere(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance, _checkLayer);
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
            if (speed <= 0.1f) _checkSpeed = 0;

            else _checkSpeed = speed;
        }

        _checkSpeed = Mathf.Clamp(_checkSpeed, 0, _maxSpeed);

        speed = _checkSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(_carFront.position, transform.forward * _maxDistance);
        Gizmos.DrawWireSphere(_carFront.position + (transform.forward * _maxDistance / 2), _maxDistance);
    }
}