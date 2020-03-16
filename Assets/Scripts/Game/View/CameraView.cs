using System;
using UnityEngine;

namespace View
{
    [Serializable]
    public struct CameraVariables
    {
        public float SlerpSpeed;
        public float TransitionSpeed;
        public float MouseRotationSpeed;

        public Vector3 Offset;
        public Vector2 VerticalLookAngle;
        public Vector2 HorizontalLookAngle;

        public PlayerView Target;
        public Camera Camera;

        public Transform ThirdPersonCameraTransform;
        public Transform FirstPersonCameraTransform;
    }

    public class CameraView : MonoBehaviour
    {
        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                transform.localPosition = value;
            }
        }

        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                transform.localRotation = value;
            }
        }

        [SerializeField] private CameraVariables _variables;
        public CameraVariables Variables => _variables;
        
        public Transform Transform => transform;
    }
}