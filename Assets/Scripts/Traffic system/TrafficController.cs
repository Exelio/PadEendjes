using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrafficController : MonoBehaviour
{
    [Header("Movement variables")]
    [Tooltip("The acceleration of the vehicle")] [SerializeField] private float _accelerationSpeed;
    [Range(1, 8)] [Tooltip("Max car speed")] [SerializeField] private float _speed;
    [Tooltip("The speed the car turns")] [SerializeField] private float _rotationSpeed;
    [Range(0,2)] [Tooltip("Distance until change waypoint")] [SerializeField] private float _distance = 0.1f;

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

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _waypoint.transform.position;
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_waypoint != null)
        {
            CheckDestinationReached();
            Move();
        }
    }

    private void Move()
    {
        Vector3 destinationDirection = _waypoint.transform.position - transform.position;
        destinationDirection.y = 0;

        if(_rigid.velocity.magnitude <= _speed)
            _rigid.AddForce(transform.forward * _accelerationSpeed * Time.deltaTime, ForceMode.Acceleration);

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
}
