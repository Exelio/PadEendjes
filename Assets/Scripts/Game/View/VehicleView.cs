using System;
using UnityEngine;

[Serializable]
public struct VehicleVariables
{
    [Header("Movement variables")]
    [Tooltip("The acceleration of the vehicle")] public float AccelerationSpeed;
    [Range(1, 15)] [Tooltip("Max car speed")] public float MaxSpeed; 
    [Tooltip("The speed the car turns")] public float RotationSpeed;
    [Tooltip("This rigidbody")] public Rigidbody RigidBody;

    [Header("Check for waypoints")]
    [Range(0, 2)] [Tooltip("Distance until change waypoint")] public float Distance;

    [Header("Field of view")]
    [Tooltip("Vehicle front position")] public Transform CarFront;

    [Tooltip("Targets to watch out for")] public LayerMask TargetMask;
    [Tooltip("Obstacles that can block the fov to a target")] public LayerMask ObstacleMask;

    [Tooltip("The angle the player must be in between to cross the road")] public Vector2 AngleMaxMin;
    [Tooltip("The distance between the cars")] public float VehicleMaxDistance;
    [Tooltip("The distance the player has to be in to let the car stop")] public float PedestrianDistance;
    [Tooltip("The max fov radius of the car")] public float ViewRadius;
    [Tooltip("The fov angle the car looks in")] [Range(0, 360)] public float ViewAngle;
}

public class VehicleView : MonoBehaviour
{
    public VehicleVariables ViewVariables => _variables;
    [SerializeField] private VehicleVariables _variables;
    [Tooltip("starting waypoint")] public Waypoint StartWaypoint;

    [SerializeField] private float _checkSpeed; public float CheckSpeed { get => _checkSpeed; set { _checkSpeed = value; } }

    public void DestroyVehicle() { Destroy(this.gameObject); }
    
    #region DrawGizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_variables.CarFront.position, _variables.ViewRadius);


        Gizmos.color = Color.red;
        Vector3 viewAngleA = DirFromAngle(-_variables.ViewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(_variables.ViewAngle / 2, false);

        Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleA * _variables.ViewRadius);
        Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleB * _variables.ViewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    #endregion
}
