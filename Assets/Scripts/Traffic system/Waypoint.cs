using System.Collections;
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
    [Tooltip("maximum speed to go to the next waypoint")]public float MaxSpeed = 2f;

    public int Number { get; set; }

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * Width / 2f;
        Vector3 maxbound = transform.position - transform.right * Width / 2f;

        return Vector3.Lerp(minBound, maxbound, Random.Range(0, 1f));
    }
}
