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

        //Vector3 dir = wp.transform.forward;
        //Vector3 pos = wp.NextWaypoint.transform.position;
        //DrawArrow(true, pos, dir, Gizmos.color);

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

    private static void DrawArrow(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
        if (gizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
        }
        else
        {
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, up * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, down * arrowHeadLength, color);
        }
    }
}