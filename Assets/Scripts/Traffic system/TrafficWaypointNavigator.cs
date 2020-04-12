using UnityEngine;

[RequireComponent(typeof(TrafficController))]
public class TrafficWaypointNavigator : MonoBehaviour
{
    [Tooltip("The wayoint to start on")] [SerializeField] private Waypoint _currentWaypoint;
    private TrafficController _tfController;

    private void Awake()
    {
        _tfController = GetComponent<TrafficController>();
        _tfController.Waypoint = _currentWaypoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (_tfController.ReachedDestination)
        {
            if (_currentWaypoint.IsDriveable)
            {
                _currentWaypoint = _currentWaypoint.NextWaypoint;
                _tfController.Waypoint = _currentWaypoint;
                _tfController.ChangeCheckSpeed();
            }
            else { _tfController.Waypoint = null;}
        }        
    }
}
