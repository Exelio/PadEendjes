using System;
using System.Collections;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct VehicleVariables
    {
        [Header("Car materials")]
        public Renderer Renderer;
        public int ChangeableMaterialNumber;
        public int RainbowCarChance;
        [Tooltip("colors the car can choose from when spawned in")] public Material[] CarColors;

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
        [Tooltip("Angle to check when pedestrian is crossing in front of the car")] public float PedestrianCrossingAngle;
        [Tooltip("Angle to check when pedestrian is in front of the car without crossing the road")] public float PedestrianInFrontAngle;
        [Tooltip("The distance between the cars")] public float VehicleMaxDistance;
        [Tooltip("The distance between the cars")] public float VehicleMinDistance;
        [Tooltip("The distance the player has to be in to let the car stop")] public float PedestrianDistance;
        [Tooltip("The max fov radius of the car")] public float ViewRadius;
        [Tooltip("The fov angle the car looks in")] [Range(0, 360)] public float ViewAngle;

        public AudioSource AudioSource;
    }

    public class VehicleView : MonoBehaviour
    {
        public VehicleVariables ViewVariables => _variables;

        [SerializeField] private float _checkSpeed; public float CheckSpeed { get => _checkSpeed; set { _checkSpeed = value; } }
        [Tooltip("starting waypoint")] public Waypoint StartWaypoint;
        [SerializeField] private bool _showDebug;

        [SerializeField] private VehicleVariables _variables;

        private bool _isRainbowCar = false;

        private void Start()
        {
            if (UnityEngine.Random.Range(1, _variables.RainbowCarChance) == 1) _isRainbowCar = true;
            ChangeMaterial();

            if (_isRainbowCar) StartCoroutine(changeMaterials(.2f));
        }

        private void ChangeMaterial()
        {
            Material[] materials = _variables.Renderer.sharedMaterials;
            materials[_variables.ChangeableMaterialNumber] = _variables.CarColors[UnityEngine.Random.Range(0, _variables.CarColors.Length)];
            _variables.Renderer.sharedMaterials = materials;
        }

        public void DestroyVehicle() { Destroy(this.gameObject); }

        private IEnumerator changeMaterials(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                ChangeMaterial();
            }
        }

        #region DrawGizmos

        private void OnDrawGizmos()
        {
            if (!_showDebug) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_variables.CarFront.position, _variables.ViewRadius);

            //fov of the vehicle
            Gizmos.color = Color.red;
            Vector3 viewAngleA = DirFromAngle(-_variables.ViewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(_variables.ViewAngle / 2, false);

            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleA * _variables.ViewRadius);
            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleB * _variables.ViewRadius);

            //pedestrian walking in front when crossing the road
            Gizmos.color = Color.blue;
            viewAngleA = DirFromAngle(_variables.PedestrianCrossingAngle, false);
            viewAngleB = DirFromAngle(-_variables.PedestrianCrossingAngle, false);

            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleA * _variables.PedestrianDistance);
            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleB * _variables.PedestrianDistance);

            //pedestrian in front check
            Gizmos.color = Color.green;
            viewAngleA = DirFromAngle(_variables.PedestrianInFrontAngle, false);
            viewAngleB = DirFromAngle(-_variables.PedestrianInFrontAngle, false);

            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleA * _variables.PedestrianDistance);
            Gizmos.DrawLine(_variables.CarFront.position, transform.position + viewAngleB * _variables.PedestrianDistance);
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
}