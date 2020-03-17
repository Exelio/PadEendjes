using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint wp, GizmoType type)
    {
        if((type & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red * 0.5f;
        }

        Gizmos.DrawSphere(wp.transform.position, .1f);
        Gizmos.color = Color.white;

        Gizmos.DrawLine(wp.transform.position + (wp.transform.right * wp.Width / 2f), wp.transform.position - (wp.transform.right * wp.Width / 2f));

        if(wp.PreviousWaypoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 offset = wp.transform.right * wp.Width / 2f;
            Vector3 offsetTo = wp.PreviousWaypoint.transform.right * wp.PreviousWaypoint.Width / 2f;

            Gizmos.DrawLine(wp.transform.position + offset, wp.PreviousWaypoint.transform.position + offsetTo);
        }
        if(wp.NextWaypoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 offset = wp.transform.right * -wp.Width / 2f;
            Vector3 offsetTo = wp.NextWaypoint.transform.right * -wp.NextWaypoint.Width / 2f;

            Gizmos.DrawLine(wp.transform.position + offset, wp.NextWaypoint.transform.position + offsetTo);
        }
        if(wp.BrancheWaypoints != null)
        {
            foreach (var branche in wp.BrancheWaypoints)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawLine(wp.transform.position, branche.transform.position);
            }
        }
    }
}
