using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint PreviousWaypoint;
    public Waypoint NextWaypoint;

    [Range(0, 5)] public float Width = 1f;

    public List<Waypoint> BrancheWaypoints;

    [Range(0, 1f)] public float BranchRatio = 0.5f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * Width / 2f;
        Vector3 maxbound = transform.position - transform.right * Width / 2f;

        return Vector3.Lerp(minBound, maxbound, Random.Range(0, 1f));
    }
}
