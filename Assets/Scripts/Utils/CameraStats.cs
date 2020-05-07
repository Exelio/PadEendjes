using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct CameraStats
    {
        [Header("Main Camera Transform")]
        [Tooltip("Assign the main camera")]
        public Transform CameraTransform;

        [Header("Anchor Point Transforms")]
        [Tooltip("Assign the camera's anchor points")]
        public Transform PrimaryAnchorPoint;

        [Space]
        public Transform ForwardAnchorPoint;
        public Transform BackwardAnchorPoint;

        [Header("Obstacle Collision Settings")]
        [Tooltip("Set minimum distance")]
        [Range(0.1f, 1.5f)]
        public float MinimumDistance;

        [Tooltip("Set maximum distance")]
        [Range(0.5f, 5f)]
        public float MaximumDistance;

        [Space]
        [Tooltip("Set collision check radius")]
        [Range(0.01f, 0.3f)]
        public float CollisionRadius;

        [Space]
        [Tooltip("Set the interpolation speed")]
        [Range(1f, 10f)]
        public float ClipLerpSpeed;

        [Header("Primary Anchor Point Settings")]
        [Tooltip("Set primary anchor point's offset")]
        public Vector3 Offset;

        [Space]
        [Tooltip("Set interpolation speed for the camera follow")]
        [Range(0.05f, 1f)]
        public float FollowLerpSpeed;

        [Tooltip("Set interpolation speed for switching camera side")]
        [Range(0.05f, 1f)]
        public float SwitchLerpSpeed;

        [Tooltip("Set rotation speed")]
        [Range(30f, 150f)]
        public float RotationSpeed;

        [Space]
        [Tooltip("Set vertical rotation clamp in degrees (lower limit, upper limit)")]
        public Vector2 PitchClamp;

        [Tooltip("Set horizontal rotation clamp in degrees (lower limit, upper limit)")]
        public Vector2 YawClamp;

        [Space]
        [Tooltip("Invert vertical controls")]
        public bool IsVerticalInverted;

        [Header("Mistake checking")]
        public LayerMask VehicleWindowLayer;
    }
}