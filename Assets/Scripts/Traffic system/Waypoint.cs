using System;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint PreviousWaypoint;
    public Waypoint NextWaypoint;
    
    public List<Waypoint> BrancheWaypoints;

    [Range(0, 5)] public float Width = 1f;
    [Tooltip("chance to go to branches in %")] [Range(0, 1f)] public float BranchRatio = 0.5f;
    [Tooltip("Editor sphere radius")] [Range(0.1f, 1)] public float SphereRadius = 0.5f;
    [Tooltip("maximum speed to go to the next waypoint")] public float MaxSpeed = 2f;

    [Tooltip("Traffic light next to the waypoint")] [SerializeField] private TrafficLightView _trafficLight;

    public int Number { get; set; }
    public bool IsDriveable = true;

    private void Start()
    {
        if (_trafficLight != null) 
        {
            _trafficLight.IsRedLight = IsDriveable;
            _trafficLight.Initialize();
            _trafficLight.OnLightChange += ChangeBool; 
        }
    }

    private void ChangeBool(bool driveable)
    {
        IsDriveable = driveable;
    }

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * Width / 2f;
        Vector3 maxbound = transform.position - transform.right * Width / 2f;

        return Vector3.Lerp(minBound, maxbound, UnityEngine.Random.Range(0, 1f));
    }
}
