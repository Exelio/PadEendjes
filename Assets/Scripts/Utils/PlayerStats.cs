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
    }
}