using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct PlayerStats
    {
        [Header("Layer Properties")]
        [Tooltip("Assign the walkable layer")]
        public LayerMask WalkableLayer;
        [Tooltip("Assign the street layer")]
        public LayerMask StreetLayer;
        [Tooltip("Assign crossing road layer")]
        public LayerMask CrossingRoadLayer;
        [Tooltip("Assign unsavespot layer")]
        public LayerMask UnsaveSpotLayer;
        [Tooltip("Assign walk Area layer")]
        public LayerMask WalkAreaLayer;

        [Header("Movement Parameters")]
        [Tooltip("Set the player's movement speed")]
        public float Speed;

        [Range(0f, 0.3f)]
        [Tooltip("Linear interpolation of the player's rotation")]
        public float RotationLerpSpeed;

        [Header("Gravity Parameters")]
        [Tooltip("Set the player's gravity")]
        public float Gravity;

        [Header("Animation Parameters")]
        [Range(2f, 8f)]
        [Tooltip("Time till player gets bored")]
        public float TimeTillBored;

        [Header("Check Parameters")]
        [Range(0f, 1f)]
        [Tooltip("Distance between player and street to change view")]
        public float DistanceToStreet;

        [Range(0, 5)]
        [Tooltip("Slight difference in crossing road angle")]
        public float MaxAngleDifference;

        [Range(5, 15)]
        [Tooltip("Radius to check for a crossing road")]
        public int MaxRadius;

        [HideInInspector]
        public AudioSource AudioSource;

        [HideInInspector]
        public Rigidbody Rigidbody;

        [HideInInspector]
        public CapsuleCollider Collider;

        [HideInInspector]
        public Transform Transform => Rigidbody.transform;

        [HideInInspector]
        public GameObject InteractableObject;

        [HideInInspector]
        public bool IsOnStreet;
        [HideInInspector]
        public bool IsOnCrossingRoad;
        [HideInInspector]
        public bool IsOnWalkingArea;
    }
}