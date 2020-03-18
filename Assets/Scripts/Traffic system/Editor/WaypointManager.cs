using UnityEditor;
using UnityEngine;

public class WaypointManager : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManager>();
    }

    public Transform WaypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("WaypointRoot"));

        if(WaypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected", MessageType.Warning);
        }

        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButtons()
    {
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if(Selection.activeObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if(GUILayout.Button("Add Branche Wapoint"))
            {
                AddBranch();
            }
            if(GUILayout.Button("Create Waypoint Before"))
            {
                CreateBefore();
            }
            if(GUILayout.Button("Create Waypoint After"))
            {
                CreateAfter();
            }
            if(GUILayout.Button("Remove Waypoint"))
            {
                Remove();
            }
        }

        for (int i = 0; i < WaypointRoot.childCount; i++)
        {
            ChangeWaypointName(WaypointRoot.GetChild(i)?.GetComponent<Waypoint>());
        }
    }

    private void ChangeWaypointName(Waypoint waypoint)
    {
        if (waypoint == null) return;

        if (waypoint.PreviousWaypoint != null && waypoint.NextWaypoint != null)
        {
            waypoint.name = "Waypoint" + waypoint.Number + " From" + waypoint.PreviousWaypoint.Number + "To" + waypoint.NextWaypoint.Number;
        }
        else if (waypoint.PreviousWaypoint != null)
        {
            waypoint.name = "Waypoint" + waypoint.Number + " From" + waypoint.PreviousWaypoint.Number;
        }
        else if(waypoint.NextWaypoint != null)
        {
            waypoint.name = "Waypoint" + waypoint.Number + " From" + waypoint.NextWaypoint.Number;
        }
        else
        {
            waypoint.name = "Waypoint" + waypoint.Number + " NO From or To";
        }
    }

    private void CreateWaypoint()
    {
        GameObject waypointObj = new GameObject("Waypoint", typeof(Waypoint));
        waypointObj.transform.SetParent(WaypointRoot, false);

        Waypoint wp = waypointObj.GetComponent<Waypoint>();
        wp.Number = WaypointRoot.childCount;

        if(WaypointRoot.childCount > 1)
        {
            wp.PreviousWaypoint = WaypointRoot.GetChild(WaypointRoot.childCount - 2).GetComponent<Waypoint>();
            wp.PreviousWaypoint.NextWaypoint = wp;

            //set the transform of the waypoint
            wp.transform.position = wp.PreviousWaypoint.transform.position;
            wp.transform.forward = wp.PreviousWaypoint.transform.forward;
        }
        Selection.activeGameObject = wp.gameObject;
    }

    private void CreateBefore()
    {
        GameObject waypointObj = new GameObject("Waypoint" + WaypointRoot.childCount, typeof(Waypoint));
        waypointObj.transform.SetParent(WaypointRoot, false);

        Waypoint wp = waypointObj.GetComponent<Waypoint>();
        wp.Number = WaypointRoot.childCount;

        Waypoint swp = Selection.activeGameObject.GetComponent<Waypoint>();

        wp.transform.position = swp.transform.position;
        wp.transform.forward = swp.transform.forward;

        if(swp.PreviousWaypoint != null)
        {
            wp.PreviousWaypoint = swp.PreviousWaypoint;
            swp.PreviousWaypoint.NextWaypoint = wp;
        }

        wp.NextWaypoint = swp;
        swp.PreviousWaypoint = wp;

        wp.transform.SetSiblingIndex(swp.transform.GetSiblingIndex());
        Selection.activeGameObject = wp.gameObject;
    }

    private void CreateAfter()
    {
        GameObject waypointObj = new GameObject("Waypoint" + WaypointRoot.childCount, typeof(Waypoint));
        waypointObj.transform.SetParent(WaypointRoot, false);

        Waypoint wp = waypointObj.GetComponent<Waypoint>();
        wp.Number = WaypointRoot.childCount;

        Waypoint swp = Selection.activeGameObject.GetComponent<Waypoint>();

        wp.transform.position = swp.transform.position;
        wp.transform.forward = swp.transform.forward;

        wp.PreviousWaypoint = swp;

        if (swp.NextWaypoint != null)
        {
            wp.NextWaypoint = swp.NextWaypoint;
            swp.NextWaypoint.PreviousWaypoint = wp;
        }

        swp.NextWaypoint = wp;

        wp.transform.SetSiblingIndex(swp.transform.GetSiblingIndex());
        Selection.activeGameObject = wp.gameObject;
    }

    private void Remove()
    {
        Waypoint wp = Selection.activeGameObject.GetComponent<Waypoint>();

        if(wp.NextWaypoint != null)
        {
            wp.NextWaypoint.PreviousWaypoint = wp.PreviousWaypoint;
        }
        if(wp.PreviousWaypoint != null)
        {
            wp.PreviousWaypoint.NextWaypoint = wp.NextWaypoint;
            Selection.activeGameObject = wp.PreviousWaypoint.gameObject;
        }
        DestroyImmediate(wp.gameObject);
    }

    private void AddBranch()
    {
        GameObject waypointObj = new GameObject("BrancheWaypoint", typeof(Waypoint));
        waypointObj.transform.SetParent(WaypointRoot, false);

        Waypoint wp = waypointObj.GetComponent<Waypoint>();
        wp.Number = WaypointRoot.childCount;

        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedFrom.BrancheWaypoints.Add(wp);

        wp.transform.position = branchedFrom.transform.position;
        wp.transform.forward = branchedFrom.transform.forward;

        Selection.activeGameObject = wp.gameObject;
    }
}
