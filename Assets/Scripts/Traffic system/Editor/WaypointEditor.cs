using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad()]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint wp, GizmoType type)
    {
        DrawReferenceLines(wp);
        DrawReferenceGizmos(wp, type);
    }

    private static void DrawReferenceLines(Waypoint wp)
    {
        if (wp.PreviousWaypoint != null)
        {
            Gizmos.color = Color.white;
            Vector3 offset = wp.transform.right * wp.Width / 2f;
            Vector3 offsetTo = wp.PreviousWaypoint.transform.right * wp.PreviousWaypoint.Width / 2f;

            Gizmos.DrawLine(wp.transform.position + offset, wp.PreviousWaypoint.transform.position + offsetTo);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(wp.transform.position, wp.PreviousWaypoint.transform.position);
        }
        if (wp.NextWaypoint != null)
        {
            Gizmos.color = Color.white;
            Vector3 offset = wp.transform.right * -wp.Width / 2f;
            Vector3 offsetTo = wp.NextWaypoint.transform.right * -wp.NextWaypoint.Width / 2f;

            Gizmos.DrawLine(wp.transform.position + offset, wp.NextWaypoint.transform.position + offsetTo);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(wp.transform.position, wp.NextWaypoint.transform.position);
        }
        if (wp.BrancheWaypoints != null)
        {
            foreach (var branche in wp.BrancheWaypoints)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawLine(wp.transform.position, branche.transform.position); 
            }
        }
    }

    private static void DrawReferenceGizmos(Waypoint wp, GizmoType type)
    {
        CheckSelected(type);

        Gizmos.DrawSphere(wp.transform.position, wp.SphereRadius);

        Gizmos.color *= 2f;
        Gizmos.DrawLine(wp.transform.position + (wp.transform.right * wp.Width / 2f), wp.transform.position - (wp.transform.right * wp.Width / 2f));
    }

    private static void CheckSelected(GizmoType type)
    {
        if ((type & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red * 0.5f;
        }
    }
}
