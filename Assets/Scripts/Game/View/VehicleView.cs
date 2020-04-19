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

    [Header("Check variables")]
    [Range(0, 2)] [Tooltip("Distance until change waypoint")] public float Distance;
    [Tooltip("The distance between the cars")] public float MaxDistance;
    [Tooltip("The max angle between the car in front and this before it isn.t in front anymore")] public float CheckAngle;
    [Tooltip("Layers to check if something is in front of the vehicle")] public LayerMask CheckLayer;
    [Tooltip("Vehicle front position")] public Transform CarFront;
}

public class VehicleView : MonoBehaviour
{
    public VehicleVariables ViewVariables => _variables;
    [SerializeField] private VehicleVariables _variables;
    [Tooltip("starting waypoint")] public Waypoint StartWaypoint;

    [SerializeField] private float _checkSpeed; public float CheckSpeed { get => _checkSpeed; set { _checkSpeed = value; } }

    public void DestroyVehicle() { Destroy(this.gameObject); }
}
