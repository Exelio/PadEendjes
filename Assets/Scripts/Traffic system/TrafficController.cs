using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrafficController : MonoBehaviour
{
    [Header("Movement variables")]
    [Tooltip("The acceleration of the vehicle")] [SerializeField] private float _accelerationSpeed;
    [Range(1, 8)] [Tooltip("Max car speed")] [SerializeField] private float _speed; public float Speed => _speed;
    [Tooltip("The speed the car turns")] [SerializeField] private float _rotationSpeed;
    [Range(0,2)] [Tooltip("Distance until change waypoint")] [SerializeField] private float _distance = 0.1f;
    [Tooltip("The distance between the cars")] [SerializeField] private float _maxDistance = 2f;
    [Tooltip("Vehicle front position")] [SerializeField] private Transform _carFront;

    private Waypoint _waypoint;
    public Waypoint Waypoint
    {
        get => _waypoint;
        set
        {
            _waypoint = value;
        }
    }

    private bool _reachedDestination = false; public bool ReachedDestination => _reachedDestination;

    private Rigidbody _rigid;

    private Waypoint _destinationWP;
    private Transform _destinationTrans;

    private float _checkSpeed;
    [SerializeField] private float _currentSpeed; public float CurrentSpeed => _currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _waypoint.transform.position;
        _checkSpeed = _speed;
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_waypoint != null)
        {
            CheckDestinationReached();
            Move();
            CheckForward();
        }
    }

    private void Move()
    {
        Vector3 destinationDirection = _waypoint.transform.position - transform.position;
        destinationDirection.y = 0;
        _currentSpeed = _rigid.velocity.magnitude;

        if (_currentSpeed <= _checkSpeed)
            _rigid.AddForce(transform.forward * _accelerationSpeed, ForceMode.Force);

        Quaternion targetRotation = Quaternion.LookRotation(destinationDirection); 
        var slerp = Quaternion.Slerp(transform.rotation,targetRotation, _rotationSpeed * Time.deltaTime);
        transform.rotation = slerp;
    }

    private void CheckDestinationReached()
    {
        if (Vector3.Distance(transform.position, _waypoint.transform.position) <= _distance)
        {
            _reachedDestination = true;
        }
        else
        {
            _reachedDestination = false;
        }
    }

    private void CheckForward()
    {
        RaycastHit hit;
        bool _hitDetection = Physics.BoxCast(_carFront.position + (_carFront.transform.forward * (_maxDistance / 2)), _carFront.localScale * _maxDistance, _carFront.forward, out hit, _carFront.rotation, _maxDistance);

        if (_hitDetection)
        {
            _checkSpeed = hit.transform.GetComponent<TrafficController>().CurrentSpeed;
        }
        else _checkSpeed = _speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_carFront.position + (_carFront.forward * _maxDistance/2), _carFront.localScale * _maxDistance);
    }
}
