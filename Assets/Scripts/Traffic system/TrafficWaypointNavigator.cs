using UnityEngine;

[RequireComponent(typeof(VehicleView))]
public class TrafficWaypointNavigator
{
    private Waypoint _currentWaypoint;
    private TrafficController _tfController;

    public TrafficWaypointNavigator(TrafficController controller, Waypoint startWaypoint)
    {
        _tfController = controller;
        _currentWaypoint = startWaypoint;
    }

    public void Initialize()
    {
        _tfController.Waypoint = _currentWaypoint;
    }

    // Update is called once per frame
    public void Update()
    {
        if (_tfController.ReachedDestination && _currentWaypoint != null)
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
