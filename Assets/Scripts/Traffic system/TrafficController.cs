using System;
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
    [Tooltip("Vehicle front position")] [SerializeField] private Transform _carFront;

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

    private float _checkSpeed;
    [SerializeField] private float _currentSpeed; public float CurrentSpeed => _currentSpeed;

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
        Vector3 degrees = wpTrans.rotation.eulerAngles;
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
        else { _checkSpeed = 0; _currentSpeed = 0; }
    }

    private void FixedUpdate()
    {
        if (_wp != null) Move();
    }

    private void Move()
    {
        Debug.Log("Move");
        Vector3 destinationDirection = _wp.transform.position - transform.position;
        destinationDirection.y = 0;
        _currentSpeed = _rigid.velocity.magnitude;

        if (_currentSpeed <= _checkSpeed) _rigid.AddForce(transform.forward * _accelerationSpeed, ForceMode.Force);

        Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
        var slerp = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.rotation = slerp;
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
        float speed = 0;
        RaycastHit hit;
        bool hitDetection = Physics.BoxCast(_carFront.position + (transform.forward * (_maxDistance / 2)), _carFront.localScale, transform.forward, out hit, _carFront.rotation, _maxDistance);

        if (hitDetection && hit.transform.GetComponent<TrafficController>() != null)
        {
            _behindVehicle = true;
            speed = hit.transform.GetComponent<TrafficController>().CurrentSpeed;
        }
        else _behindVehicle = false;

        if(_checkSpeed != speed)
            ChangeSpeed(speed);
    }

    private void ChangeSpeed(float speed)
    {
        if (_behindVehicle)
        {
            if (speed <= 0.1f) _checkSpeed = 0;

            else _checkSpeed = speed;
        }
        else _checkSpeed = _wp.PreviousWaypoint.MaxSpeed;

        _checkSpeed = Mathf.Clamp(_checkSpeed, 0, _maxSpeed);
    }
}